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

    public Notification CreateNotificationForNewWinningBid(Auction auction, User user)
    {
        var notification = CreateBasicNotification(auction, user);

        notification.Description = "You currently have the highest bid of " + auction.CurrentPrice +
                                   " for the item: " + auction.GetItemName() + ".";

        return notification;
    }

    public Notification CreateNotificationForSuccessfullyAddedAuction(Auction auction, User user)
    {
        var notification = CreateBasicNotification(auction, user);
        notification.Description = auction.GetItemName() + " was successfully put for auction, " +
                                   "with the starting price of: " + auction.CurrentPrice +
                                   " and the minimum bid increment of: " + auction.MinimumBidIncrement;

        return notification;
    }
}