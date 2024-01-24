﻿using System.Security.Claims;
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
        IUsersService userService, INotificationService notificationService, IJwtService jwtService,
        IUserContextService userContextService)
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

        //TODO for development purposes, the functionality of choosing the ending time is commented out.
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
        auction.SetWinningBidId(existingUserBid.Id);
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
        await _context.SaveChangesAsync();
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

        var user = await _userService.QueryProfileAccount();

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
            return auction;
        }
        winningBid.Status = BidStatus.Win;

        var winningUser = await _userService.QueryUserById(winningBid.UserId);
        winningUser.PayWithFrozenCredit(winningBid.Amount);
        auction.SetWinningBidId(winningBid.Id);

        _notificationService.HandleNotificationForWinner(auction, winningUser);

        await _userService.HandleLosingBids(auction);
        
        await HandlePayment(auction, seller);

        winningUser.AddItem(item);
        item.AvailableForAuction = true;

        _context.Items.Update(item);
        _context.Bids.Update(winningBid);
        _context.Users.Update(winningUser);
        await _context.SaveChangesAsync();

        return auction;
    }

    private async Task HandlePayment(Auction auction, User seller)
    {
        var ownerAccount = await _userService.QueryOwner();
        var taxes = auction.CurrentPrice / 20;

        seller.Sell(auction, taxes);
        ownerAccount.Credit += taxes;
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