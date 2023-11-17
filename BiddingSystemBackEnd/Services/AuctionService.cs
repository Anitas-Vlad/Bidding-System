﻿using BiddingSystem.Context;
using BiddingSystem.Models;
using BiddingSystem.Models.Enums;
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

    public async Task<Auction> PlaceBid(CreateBiddingRequest request)
    {
        var user = await _userService.QueryUserById(request.UserId);
        var auction = await QueryAuctionById(request.AuctionId);

        auction.IsBidAmountValid(request.Amount);

        var optionalPreviousWinningBids = auction.Bids.Where(bid => bid.Status == BidStatus.Winning).ToList();
        var optionalPreviousUserBid = auction.GetBidByUserId(request.UserId);

        if (optionalPreviousUserBid == null)
        {
            user.CheckIfHasEnoughCredit(auction.MinimumBidIncrement);

            foreach (var previousWinningBid in optionalPreviousWinningBids)
            {
                previousWinningBid.Status = BidStatus.Losing;
                _context.Bids.Update(previousWinningBid);
            }

            var bid = _biddingService.ConstructBid(request);
            user.FreezeCredit(request.Amount);
            user.AddBid(bid);
            auction.AddBid(bid);

            _context.Bids.Add(bid);
            _context.Users.Update(user);
            _context.Auctions.Update(auction);
            await _context.SaveChangesAsync();

            return auction;
        }

        var differenceBetweenOldAndNewAmount =
            optionalPreviousUserBid.GetDifferenceBetweenOldAndNewAmount(request.Amount);

        user.CheckIfHasEnoughCredit(differenceBetweenOldAndNewAmount);
        
        foreach (var previousWinningBid in optionalPreviousWinningBids)
        {
            previousWinningBid.Status = BidStatus.Losing;
            _context.Bids.Update(previousWinningBid);
        }

        user.FreezeCredit(differenceBetweenOldAndNewAmount);
        optionalPreviousUserBid.UpdateAmount(request.Amount);

        _context.Bids.Update(optionalPreviousUserBid);
        _context.Users.Update(user);
        _context.Auctions.Update(auction);
        await _context.SaveChangesAsync();

        return auction;
    }

    private void UpdateChangesAfterPlacingBid(Bid? previousWinningBid, User user, Auction auction)
    {
        _context.Bids.Update(previousWinningBid);
        _context.Users.Update(user);
        _context.Auctions.Update(auction);
    }

    public async Task<Auction> CancelBid(int bidId)
    {
        var bid = await _biddingService.QueryBidById(bidId);
        var auction = await QueryAuctionById(bid.AuctionId);
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

    public void HandleLosingBids()
    {
        
    }

    // public async Task<Auction?> EndAuction(int auctionId)
    // {
    //     var auction = await QueryAuctionById(auctionId);
    //     var winningBid = auction.GetWinningBid(); //TODO THIS CAN BE NULL IF NOBODY BID ON IT
    //     
    //     if (winningBid == null)
    //     {
    //         auction.EndAuctionWithNoWinnder();
    //         return .....;
    //     }
    //
    //     var WinningUser = await _userService.QueryUserById(winningBid.UserId);
    //
    //     winningBid.Status = BidStatus.Win;
    //
    //     foreach (var bid in auction.Bids.Where(bid => bid.Status == BidStatus.Losing))
    //     {
    //         bid.Status = BidStatus.Loss;
    //         var losingBidUser = await _userService.QueryUserById(bid.UserId);
    //         losingBidUser.UnfreezeCredit(bid.Amount);
    //
    //         _context.Bids.Update(bid);
    //         _context.Users.Update(losingBidUser);
    //     }
    //
    //     WinningUser.Pay(winningBid.Amount);
    //
    //     _context.Bids.Update(winningBid);
    //     _context.Users.Update(WinningUser);
    //     await _context.SaveChangesAsync();
    //
    //     return winningBid;
    // }
}