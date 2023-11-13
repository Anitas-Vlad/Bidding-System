using BiddingSystem.Context;
using BiddingSystem.Models;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiddingSystem.Services;

public class AuctionService : IAuctionService
{
    private readonly BiddingSystemContext _context;
    private readonly IItemService _itemService;
    private readonly IBiddingService _biddingService;
    private readonly IUsersService _userService;

    public AuctionService(BiddingSystemContext context, IItemService itemService, IBiddingService biddingService,
        IUsersService userService)
    {
        _context = context;
        _itemService = itemService;
        _biddingService = biddingService;
        _userService = userService;
    }

    public async Task<Auction> QueryAuctionById(int auctionId)
    {
        var auction =
            await _context.Auctions
                .Include(auction => auction.Item)
                .Include(auction => auction.Bids)
                .Where(auction => auction.Id == auctionId).SingleOrDefaultAsync();

        if (auction == null) throw new ArgumentException("Auction not Found.");

        return auction;
    }

    public async Task<List<Auction>> QueryAllAuctions()
        => await _context.Auctions
            .Include(auction => auction.Item)
            .Include(auction => auction.Bids)
            .ToListAsync();

    private static bool IsEndOfAuctionValid(DateTime endOfAuctionRequest)
        => endOfAuctionRequest > DateTime.Now;

    public async Task<Auction> CreateAuction(CreateAuctionRequest request)
    {
        var item = await _itemService.QueryItemById(request.ItemId);
        if (item.AvailableForAuction == false)
            throw new ArgumentException("The Item is already in auction.");

        if (!IsEndOfAuctionValid(request.EndOfAuction))
            throw new ArgumentException("The end of Auction is not valid.");

        var auction = new Auction
        {
            Item = item,
            EndOfAuction = request.EndOfAuction,
            CurrentPrice = item.StartingPrice,
            MinimumBidIncrement = request.MinimumBidIncrement
        };
        item.AvailableForAuction = false;

        _context.Items.Update(item);
        _context.Auctions.Add(auction);
        await _context.SaveChangesAsync();

        return auction;
    }

    public async Task<Auction> PlaceBiddingForAuction(CreateBiddingRequest request)
    {
        var user = await _userService.QueryUserById(request.UserId);
        var auction = await QueryAuctionById(request.AuctionId);

        if (!auction.IsBidAmountValid(request.Amount))
            throw new ArgumentException(
                "The bidding amount is not valid. Check if you respect the MinimumBidIncrement");

        var optionalPreviousBidding = auction.GetBidByUserId(request.UserId);
        
        if (optionalPreviousBidding == null)
        {
            if (!user.HasEnoughCredit(auction.MinimumBidIncrement))
                throw new ArgumentException("Not enough credit to bid at this auction.");

            var bidding = _biddingService.ConstructBid(request);
            
            user.FreezeCredit(request.Amount);
            user.AddBid(bidding);
            auction.AddBid(bidding);

            _context.Bids.Add(bidding);
            _context.Users.Update(user);
            _context.Auctions.Update(auction);
            await _context.SaveChangesAsync();
            return auction;
        }

        var differenceBetweenOldAndNewAmount =
            optionalPreviousBidding.GetDifferenceBetweenOldAndNewAmount(request.Amount);
        
        if (!user.HasEnoughCredit(differenceBetweenOldAndNewAmount))
            throw new ArgumentException("Not enough credit to bid at this auction.");
        
        user.FreezeCredit(differenceBetweenOldAndNewAmount);
        optionalPreviousBidding.UpdateAmount(request.Amount);
            
        _context.Bids.Update(optionalPreviousBidding);
        _context.Auctions.Update(auction);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return auction;
    }

    public async Task<Auction> CancelBidding(int biddingId)
    {
        var bidding = await _biddingService.QueryBidById(biddingId);
        var auction = await QueryAuctionById(bidding.AuctionId);
        var user = await _userService.QueryUserById(bidding.UserId);

        user.RemoveBid(bidding);
        auction.RemoveBid(bidding);

        _context.Users.Update(user);
        _context.Auctions.Update(auction);
        _context.Bids.Remove(bidding);

        await _context.SaveChangesAsync();
        return auction;
    }
}