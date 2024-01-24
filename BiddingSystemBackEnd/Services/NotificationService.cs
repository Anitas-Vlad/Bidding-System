using BiddingSystem.Context;
using BiddingSystem.Models;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiddingSystem.Services;

public class NotificationService : INotificationService
{
    private readonly BiddingSystemContext _context;
    private readonly IUserContextService _userContextService;

    public NotificationService(BiddingSystemContext context, IUserContextService userContextService)
    {
        _context = context;
        _userContextService = userContextService;
    }

    public async Task<List<Notification>> QueryProfileNotifications()
    {
        var userProfileId = _userContextService.GetUserId();
        return await _context.Notifications.Where(notification => notification.UserId == userProfileId)
            .OrderBy(notification => notification.IsSeen).ToListAsync();
    }

    private static Notification CreateBasicNotification(Auction auction, User user)
        => new()
        {
            UserId = user.Id,
            Title = auction.GetItemName(),
            Time = DateTime.Now.ToLocalTime()
        };

    public void HandleNotificationForNewWinningBid(Auction auction, User user)
    {
        var notification = CreateBasicNotification(auction, user);

        notification.Description = "Congratulations! You currently have the highest bid of " + auction.CurrentPrice +
                                   " for the item: " + auction.GetItemName() + ".";

        user.ReceiveNotification(notification);
        _context.Notifications.Add(notification);
    }

    public void HandleNotificationForSuccessfullyAddedAuction(Auction auction, User user)
    {
        var notification = CreateBasicNotification(auction, user);
        notification.Description = auction.GetItemName() + " was successfully put for auction, " +
                                   "with the starting price of: " + auction.CurrentPrice +
                                   " and the minimum bid increment of: " + auction.MinimumBidIncrement;

        user.ReceiveNotification(notification);
        _context.Notifications.Add(notification);
    }

    public void HandleNotificationForDowngradeToLosingBid(Auction auction, User user, double amount)
    {
        var notification = CreateBasicNotification(auction, user);

        notification.Description =
            $"Someone placed the bid of {amount}," +
            $" and is currently the winning the auction for the item: {auction.GetItemName()}";

        user.ReceiveNotification(notification);
        _context.Notifications.Add(notification);
    }
    
    public void HandleNotificationForWinner(Auction auction, User user)
    {
        var notification = CreateBasicNotification(auction, user);

        notification.Description =
            $"Congratulations, you have won the item: {auction.GetItemName()}," +
            $" and you have paid a total of: {auction.CurrentPrice}.";

        user.ReceiveNotification(notification);

        user.ReceiveNotification(notification);
        _context.Notifications.Add(notification);
    }
    
    //TODO Maybe for Losers and List<User> is needed.
    public async Task HandleNotificationForLosers(Auction auction)
    {
        var bids = auction.Bids;

        foreach (var bid in bids)
        {
            var user = await _context.Users.FirstAsync(user => user.Id == bid.UserId);
            var notification = CreateBasicNotification(auction, user);
            notification.Description = "";

            user.ReceiveNotification(notification);
            _context.Notifications.Add(notification);
        }
    }
    
    public async Task HandleNotificationForLoser(Auction auction, Bid bid)
    {
        
            var user = await _context.Users.FirstAsync(user => user.Id == bid.UserId);
            var notification = CreateBasicNotification(auction, user);
            notification.Description = "You lost in auction for the item: " + auction.GetItemName()+ 
                                       " and the amount of ______ has been unfrozen.";

            user.ReceiveNotification(notification);
            _context.Notifications.Add(notification);
    }
    
    public void HandleNotificationForSuccessfulSeller(Auction auction, User user)
    {
        var notification = CreateBasicNotification(auction, user);

        notification.Description =
            $"Congratulations! The item: {auction.GetItemName()} " +
            $"was sold at auction with the price of: {auction.CurrentPrice}.";

        user.ReceiveNotification(notification);
        _context.Notifications.Add(notification);
    }
    
    public void HandleNotificationForUnsuccessfulSeller(Auction auction, User user)
    {
        var notification = CreateBasicNotification(auction, user);

        notification.Description = $"The item: {auction.GetItemName()} was not sold at auction.";

        user.ReceiveNotification(notification);
        _context.Notifications.Add(notification);
    }
}