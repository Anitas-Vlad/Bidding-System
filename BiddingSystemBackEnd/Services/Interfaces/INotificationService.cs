using BiddingSystem.Models;
using BiddingSystem.Models.Requests;

namespace BiddingSystem.Services.Interfaces;

public interface INotificationService
{
    Task<List<Notification>> QueryProfileNotifications();
    Notification CreateNotificationForNewWinningBid(Auction auction, User user);
    Notification CreateNotificationForSuccessfullyAddedAuction(Auction auction, User user);
}