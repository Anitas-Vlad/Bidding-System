using BiddingSystem.Context;
using BiddingSystem.Models;
using BiddingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiddingSystem.Services;

public class NotificationService : INotificationService
{
    private readonly BiddingSystemContext _context;
    private readonly IUserContextService _userContextService;
    private readonly IUsersService _usersService;

    public NotificationService(BiddingSystemContext context, IUserContextService userContextService,
        IUsersService usersService)
    {
        _context = context;
        _userContextService = userContextService;
        _usersService = usersService;
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

        notification.Description =
            $"Congratulations! You currently have the highest bid of {auction.CurrentPrice}" +
            $" for the item: {auction.GetItemName()}.";

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
            $" and you have paid: {auction.CurrentPrice}.";

        user.ReceiveNotification(notification);

        user.ReceiveNotification(notification);
        _context.Notifications.Add(notification);
    }

    public async Task HandleNotificationForLoser(Auction auction, Bid bid)
    {
        var user = await _context.Users.FirstAsync(user => user.Id == bid.UserId);
        var notification = CreateBasicNotification(auction, user);
        notification.Description = $"You lost in auction for the item: {auction.GetItemName()}," +
                                   $" and the amount of {bid.Amount} has been unfrozen.";

        user.ReceiveNotification(notification);
        _context.Notifications.Add(notification);
    }

    public void HandleNotificationForSuccessfulSeller(Auction auction, User user, double taxes)
    {
        var notification = CreateBasicNotification(auction, user);

        notification.Description =
            $"Congratulations! The item: {auction.GetItemName()} " +
            $"was sold at auction with the price of: {auction.CurrentPrice} and you paid 5% taxes: {taxes}";

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

    public async Task HandleNotificationForAppOwner(Auction auction, double taxes)
    {
        var owner = await _usersService.QueryOwner();
        var notification = CreateBasicNotification(auction, owner);

        notification.Description =
            $"The item: {auction.GetItemName()} " +
            $"was sold at auction with the price of: {auction.CurrentPrice}. and the seller payed taxes of: {taxes}.";

        owner.ReceiveNotification(notification);
        _context.Notifications.Add(notification);
    }

    public void HandleNotificationForCanceledBid(Auction auction, User user)
    {
        var notification = CreateBasicNotification(auction, user);

        notification.Description =
            $"You have successfully canceled your bid at the auction for item: {auction.GetItemName()}"
            + $"and the credit of {auction.GetBidAmountByUserId(user.Id)} has been unfrozen.";

        user.ReceiveNotification(notification);
        _context.Notifications.Add(notification);
    }

    public void HandleNotificationForUpgradeToWinningBid(Auction auction, User user)
    {
        var notification = CreateBasicNotification(auction, user);

        notification.Description =
            $"Someone canceled their bid and now yours of {auction.GetBidAmountByUserId(user.Id)} is the winning bid.";

        user.ReceiveNotification(notification);
        _context.Notifications.Add(notification);
    }
}