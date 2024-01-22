using System.Security.Claims;
using BiddingSystem.Context;
using BiddingSystem.Models;
using BiddingSystem.Models.Enums;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BiddingSystem.Services;

public class AuctionService : IAuctionService
{
    private readonly BiddingSystemContext _context;
    private readonly IItemService _itemService;
    private readonly IBiddingService _biddingService;
    private readonly IUsersService _userService;
    private readonly INotificationService _notificationService;
    private readonly IJwtService _jwtService;
    private readonly IUserContextService _userContextService;

    public AuctionService(BiddingSystemContext context, IItemService itemService, IBiddingService biddingService,
        IUsersService userService, INotificationService notificationService, IJwtService jwtService, IUserContextService userContextService)
    {
        _context = context;
        _itemService = itemService;
        _biddingService = biddingService;
        _userService = userService;
        _notificationService = notificationService;
        _jwtService = jwtService;
        _userContextService = userContextService;
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

    public async Task<Auction> CreateAuction(CreateAuctionRequest request)
    {
        var item = await _itemService.QueryItemById(request.ItemId);
        
        if (item.AvailableForAuction == false)
            throw new ArgumentException("The Item is already in auction.");
        
        var seller = await _userService.QueryProfileAccount();

        if (item.UserId != seller.Id)
            throw new ArgumentException("You do not own this item.");

        // if (!IsEndOfAuctionValid(request.EndOfAuction))
        //     throw new ArgumentException("The end of Auction is not valid.");

        var auction = new Auction
        {
            Item = item,
            SellerId = seller.Id,
            EndOfAuction = DateTime.Now.ToLocalTime().AddMinutes(2),
            CurrentPrice = request.StartingPrice,
            MinimumBidIncrement = request.MinimumBidIncrement
        };
        
        item.AvailableForAuction = false;

        var notification = _notificationService.CreateNotificationForSuccessfullyAddedAuction(auction, seller);
        seller.Notifications.Add(notification);

        _context.Users.Update(seller);
        _context.Items.Update(item);
        _context.Auctions.Add(auction);
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        var endAuctionSchedule = BackgroundJob.Schedule(() => EndAuction(auction.Id), auction.EndOfAuction);

        return auction;
    }

    public async Task<Auction> PlaceBid(CreateBidRequest request, ClaimsPrincipal userClaims)
    {
        var auction = await QueryAuctionById(request.AuctionId);
        IsBeforeAuctionEndDate(auction);
        auction.IsBidAmountValid(request.Amount);

        var userProfileId = _userContextService.GetUserId();
        
        var user = await _userService.QueryUserById(userProfileId);
        var previouslyWinningBid = auction.GetWinningBid();
        var optionalPreviousUserBid = auction.GetBidByUserId(userProfileId);

        if (optionalPreviousUserBid == null)
        {
            user.CheckIfHasEnoughCredit(auction.MinimumBidIncrement);

            if (previouslyWinningBid != null)
            {
                previouslyWinningBid.Status = BidStatus.Losing;
                _context.Bids.Update(previouslyWinningBid);
            }

            var bid = _biddingService.ConstructBid(request);
            user.FreezeCredit(request.Amount);
            user.AddBid(bid);
            auction.AddBid(bid);

            var notification = _notificationService.CreateNotificationForNewWinningBid(auction, user);

            _context.Bids.Add(bid);
            _context.Users.Update(user);
            _context.Auctions.Update(auction);
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return auction;
        }

        var differenceBetweenOldAndNewAmount =
            optionalPreviousUserBid.GetDifferenceBetweenOldAndNewAmount(request.Amount);

        user.CheckIfHasEnoughCredit(differenceBetweenOldAndNewAmount);

        if (previouslyWinningBid != null)
        {
            previouslyWinningBid.Status = BidStatus.Losing;
            _context.Bids.Update(previouslyWinningBid);
        }

        user.FreezeCredit(differenceBetweenOldAndNewAmount);
        optionalPreviousUserBid.UpdateAmount(request.Amount);
        optionalPreviousUserBid.Status = BidStatus.Winning;

        _context.Bids.Update(optionalPreviousUserBid);
        _context.Users.Update(user);
        _context.Auctions.Update(auction);
        await _context.SaveChangesAsync();

        return auction;
    }

    private void CheckIfUserOwnsBid(Bid bid)
    {
        var userProfileId = _userContextService.GetUserId();

        var isSameUserId = userProfileId != bid.UserId;
        
        if (!isSameUserId)
            throw new InvalidOperationException("Invalid user ID claim.");
    }

    public async Task<Auction> CancelBid(int bidId)
    {
        var bid = await _biddingService.QueryBidById(bidId);
        var auction = await QueryAuctionById(bid.AuctionId);
        
        CheckIfUserOwnsBid(bid);
        
        var user = await _userService.QueryUserById(bid.UserId);
        
        user.CancelBid(bid);

        if (!auction.CheckIfBidToRemoveIsTheHighest(bid))
        {
            auction.RemoveLosingBid(bid);
            bid.Status = BidStatus.Cancelled;
        }
        else
        {
            var optionalNewWinningBid = auction.RemoveWinningBid(bid);
            if (optionalNewWinningBid != null)
                _context.Bids.Update(optionalNewWinningBid);
        }

        _context.Users.Update(user);
        _context.Auctions.Update(auction);
        _context.Bids.Update(bid);

        await _context.SaveChangesAsync();
        return auction;
    }

    public async Task HandleLosingBids(List<Bid> losingBids)
    {
        foreach (var bid in losingBids)
        {
            bid.Status = BidStatus.Loss;
            
            var user = await _userService.QueryUserById(bid.UserId);
            user.LoseBid(bid);
            
            _context.Users.Update(user);
            _context.Bids.Update(bid);
        }
    }

    public async Task<Auction> EndAuction(int auctionId)
    {
        var auction = await QueryAuctionById(auctionId);
        var seller = await _userService.QueryUserById(auction.SellerId);
        var item = auction.Item;

        if (auction.Bids.IsNullOrEmpty())
        {
            item.AvailableForAuction = true;

            _context.Items.Update(item);
            await _context.SaveChangesAsync();

            return auction;
        }

        var winningBid = auction.GetWinningBid();
        var losingBids = auction.Bids.Where(bid => bid.Status == BidStatus.Losing).ToList();
        
        if (winningBid == null)
        {
            await HandleLosingBids(losingBids);
            item.AvailableForAuction = true;

            _context.Items.Update(item);
            
            await _context.SaveChangesAsync();
            return auction;
        }
        
        winningBid.Status = BidStatus.Win;
        
        var winningUser = await _userService.QueryUserById(winningBid.UserId);
        winningUser.Pay(winningBid.Amount);
        
        await HandleLosingBids(losingBids);
        
        seller.SellItem(auction);
        winningUser.AddItem(item);
        item.AvailableForAuction = true;

        _context.Items.Update(item);
        _context.Bids.Update(winningBid);
        _context.Users.Update(winningUser);
        await _context.SaveChangesAsync();

        return auction;
    }
    
    private static bool IsEndOfAuctionValid(DateTime endOfAuctionRequest)
        => endOfAuctionRequest.ToLocalTime() > DateTime.Now.ToLocalTime();
    
    private static void IsBeforeAuctionEndDate(Auction auction)
    {
        if ( DateTime.Now.ToLocalTime() > auction.EndOfAuction.ToLocalTime())
            throw new Exception("Auction has ended.");
    }
}