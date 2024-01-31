using BiddingSystem.Models;

namespace BiddingSystem.Services.Interfaces;

public interface INotificationService
{
    Task<List<Notification>> QueryProfileNotifications();
    void HandleNotificationForNewWinningBid(Auction auction, User user);
    void HandleNotificationForSuccessfullyAddedAuction(Auction auction, User user);
    void HandleNotificationForDowngradeToLosingBid(Auction auction, User user, double amount);
    void HandleNotificationForWinner(Auction auction, User user);
    void HandleNotificationForSuccessfulSeller(Auction auction, User user, double taxes);
    void HandleNotificationForUnsuccessfulSeller(Auction auction, User user);
    Task HandleNotificationForLoser(Auction auction, Bid bid);
    Task HandleNotificationForAppOwner(Auction auction, double taxes);
    void HandleNotificationForCanceledBid(Auction auction, User user);
    void HandleNotificationForUpgradeToWinningBid(Auction auction, User user);
}