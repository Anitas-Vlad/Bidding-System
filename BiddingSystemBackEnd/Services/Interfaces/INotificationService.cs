using BiddingSystem.Models;
using BiddingSystem.Models.Requests;

namespace BiddingSystem.Services.Interfaces;

public interface INotificationService
{
    Task<List<Notification>> QueryProfileNotifications();
    void HandleNotificationForNewWinningBid(Auction auction, User user);
    void HandleNotificationForSuccessfullyAddedAuction(Auction auction, User user);
    void HandleNotificationForDowngradeToLosingBid(Auction auction, User user, double amount);
    void HandleNotificationForWinner(Auction auction, User user);
    void HandleNotificationForSuccessfulSeller(Auction auction, User user);
    void HandleNotificationForUnsuccessfulSeller(Auction auction, User user);
    Task HandleNotificationForLosers(Auction auction);
    Task HandleNotificationForLoser(Auction auction, Bid bid);
}