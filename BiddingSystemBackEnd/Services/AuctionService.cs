using BiddingSystem.Context;
using BiddingSystem.Models;
using BiddingSystem.Models.Enums;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace BiddingSystem.Services;

public class AuctionService : IAuctionService
{
    private readonly BiddingSystemContext _context;
    private readonly IItemService _itemService;
    private readonly IBiddingService _biddingService;
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly IUserContextService _userContextService;

    public AuctionService(BiddingSystemContext context, IItemService itemService, IBiddingService biddingService,
        IUserService userService, INotificationService notificationService,
        IUserContextService userContextService)
    {
        _context = context;
        _itemService = itemService;
        _biddingService = biddingService;
        _userService = userService;
        _notificationService = notificationService;
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
            .OrderBy(auction => auction.EndOfAuction)
            .ToListAsync();

    public async Task<Auction> CreateAuction(CreateAuctionRequest request)
    {
        var item = await _itemService.QueryItemById(request.ItemId);

        if (item.AvailableForAuction == false)
            throw new ArgumentException("The Item is already in auction.");

        var seller = await _userService.QueryPersonalAccount();

        if (item.UserId != seller.Id)
            throw new ArgumentException("You do not own this item.");

        //TODO for development purposes, the functionality of choosing the ending time is commented out.
        // if (!IsEndOfAuctionValid(request.EndOfAuction))
        //     throw new ArgumentException("The end of Auction is not valid.");

        var auction = new Auction
        {
            Item = item,
            SellerId = seller.Id,
            EndOfAuction = DateTime.Now.ToLocalTime().AddMinutes(5),
            StartingPrice = request.StartingPrice,
            CurrentPrice = request.StartingPrice,
            MinimumBidIncrement = request.MinimumBidIncrement
        };

        item.AvailableForAuction = false;
        seller.AddAuction(auction);

        _notificationService.HandleNotificationForSuccessfullyAddedAuction(auction, seller);

        _context.Users.Update(seller);
        _context.Items.Update(item);
        _context.Auctions.Add(auction);
        await _context.SaveChangesAsync();

        var endAuctionSchedule = BackgroundJob.Schedule(() => EndAuction(auction.Id), auction.EndOfAuction);

        return auction;
    }

    public async Task<Auction> PlaceBid(CreateBidRequest request)
    {
        var auction = await QueryAuctionById(request.AuctionId);
        IsBeforeAuctionEndDate(auction);
        auction.IsBidAmountValid(request.Amount);

        var userProfileId = _userContextService.GetUserId();
        var user = await _userService.QueryUserById(userProfileId);

        var previouslyWinningBid = auction.GetWinningBid();
        var optionalExistingUserBid = auction.GetBidByUserId(userProfileId);

        if (optionalExistingUserBid == null)
        {
            await HandleNewBid(auction, user, previouslyWinningBid, request);
        }
        else
        {
            await HandleExistingBid(auction, user, previouslyWinningBid, optionalExistingUserBid, request);
        }

        return auction;
    }

    private async Task HandleExistingBid(Auction auction, User user, Bid? previouslyWinningBid,
        Bid existingUserBid, CreateBidRequest request)
    {
        var differenceBetweenOldAndNewAmount =
            existingUserBid.GetDifferenceBetweenOldAndNewAmount(request.Amount);

        user.CheckIfHasEnoughCredit(differenceBetweenOldAndNewAmount);

        if (previouslyWinningBid != null)
            await HandlePreviousWinningBid(auction, previouslyWinningBid, request.Amount);

        user.FreezeCredit(differenceBetweenOldAndNewAmount);
        existingUserBid.UpdateAmount(request.Amount);
        auction.CurrentPrice = request.Amount;
        auction.WinningBidId = existingUserBid.Id;
        await SetBidStatus(existingUserBid, BidStatus.Winning);

        _notificationService.HandleNotificationForNewWinningBid(auction, user);

        _context.Bids.Update(existingUserBid);
        await UpdateDatabase(auction, user);
    }

    private async Task HandleNewBid(Auction auction, User user, Bid? previouslyWinningBid, CreateBidRequest request)
    {
        user.CheckIfHasEnoughCredit(auction.MinimumBidIncrement);

        if (previouslyWinningBid != null)
            await HandlePreviousWinningBid(auction, previouslyWinningBid, request.Amount);

        var bid = await _biddingService.CreateBid(request);
        user.FreezeCredit(request.Amount);
        user.AddBid(bid);
        auction.AddBid(bid);

        _notificationService.HandleNotificationForNewWinningBid(auction, user);

        await UpdateDatabase(auction, user);
    }

    private async Task HandlePreviousWinningBid(Auction auction, Bid previouslyWinningBid, double newAmount)
    {
        var previousWinner = await _userService.QueryUserById(previouslyWinningBid.UserId);

        _notificationService.HandleNotificationForDowngradeToLosingBid(auction, previousWinner, newAmount);

        previouslyWinningBid.Status = BidStatus.Losing;

        _context.Bids.Update(previouslyWinningBid);
        _context.Users.Update(previousWinner);
    }

    private async Task SetBidStatus(Bid bid, BidStatus status)
    {
        bid.Status = status;
        _context.Bids.Update(bid);
    }

    private async Task UpdateDatabase(Auction auction, User user)
    {
        _context.Users.Update(user);
        _context.Auctions.Update(auction);

        await _context.SaveChangesAsync();
    }

    public async Task<Auction> CancelBid(int bidId)
    {
        var bid = await _biddingService.QueryBidById(bidId);
        var auction = await QueryAuctionById(bid.AuctionId);

        _userService.CheckIfUserOwnsBid(bid);

        var user = await _userService.QueryPersonalAccount();

        user.CancelBid(bid);
        _notificationService.HandleNotificationForCanceledBid(auction, user);

        var optionalNewWinningBid = auction.SetNewHighestBid();
        if (optionalNewWinningBid != null)
        {
            var newWinner = await _userService.QueryUserById(optionalNewWinningBid.UserId);
            _notificationService.HandleNotificationForUpgradeToWinningBid(auction, newWinner);

            _context.Bids.Update(optionalNewWinningBid);
        }

        _context.Users.Update(user);
        _context.Auctions.Update(auction);
        _context.Bids.Update(bid);

        await _context.SaveChangesAsync();
        return auction;
    }

    public async Task<Auction> EndAuction(int auctionId)
    {
        var auction = await QueryAuctionById(auctionId);
        var seller = await _userService.QueryUserById(auction.SellerId);
        var item = auction.Item;

        var winningBid = auction.GetWinningBid();
        if (winningBid == null)
        {
            await HandleNoWinningBidCase(item);
            _notificationService.HandleNotificationForUnsuccessfulSeller(auction, seller);
            await _context.SaveChangesAsync();
            return auction;
        }

        winningBid.Status = BidStatus.Win;

        var winningUser = await _userService.QueryUserById(winningBid.UserId);
        winningUser.PayWithFrozenCredit(winningBid.Amount);

        var taxes = auction.CurrentPrice / 20;

        await HandleLosingBids(auction);
        await HandlePayment(auction, seller, taxes);
        await _notificationService.HandleNotificationForAppOwner(auction, taxes);

        winningUser.AddItem(item);
        item.AvailableForAuction = true;

        _notificationService.HandleNotificationForSuccessfulSeller(auction, seller, taxes);
        _notificationService.HandleNotificationForWinner(auction, winningUser);

        _context.Items.Update(item);
        _context.Bids.Update(winningBid);
        _context.Users.Update(winningUser);
        await _context.SaveChangesAsync();

        return auction;
    }

    private async Task HandlePayment(Auction auction, User seller, double taxes)
    {
        var appOwner = await _userService.QueryOwner();

        seller.Sell(auction, taxes);
        appOwner.Credit += taxes;
    }

    public async Task HandleLosingBids(Auction auction)
    {
        var losingBids = auction.GetLosingBids();
        foreach (var bid in losingBids)
        {
            var user = await _userService.QueryUserById(bid.UserId);
            user.LoseBid(bid);

            await _notificationService.HandleNotificationForLoser(auction, bid);

            _context.Users.Update(user);
            _context.Bids.Update(bid);
        }
    }

    private async Task HandleNoWinningBidCase(Item item)
    {
        item.AvailableForAuction = true;

        _context.Items.Update(item);
        await _context.SaveChangesAsync();
    }

    private static bool IsEndOfAuctionValid(DateTime endOfAuctionRequest)
        => endOfAuctionRequest.ToLocalTime() > DateTime.Now.ToLocalTime();

    private static void IsBeforeAuctionEndDate(Auction auction)
    {
        if (DateTime.Now.ToLocalTime() > auction.EndOfAuction.ToLocalTime())
            throw new Exception("Auction has ended.");
    }
}