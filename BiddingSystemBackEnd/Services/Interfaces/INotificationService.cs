using BiddingSystem.Models;
using BiddingSystem.Models.Requests;

namespace BiddingSystem.Services.Interfaces;

public interface INotificationService
{
    Task<List<Notification>> QueryProfileNotifications();
    void HandleNotificationForNewWinningBid(Auction auction, User user);
    void HandleNotificationForSuccessfullyAddedAuction(Auction auction, User user);
    void HandleNotificationForDowngradeToLosingBid(Auction auction, User user, double amount);
    void HandleNotificationForWinner(Auction auction, User user, double taxes);
    void HandleNotificationForSuccessfulSeller(Auction auction, User user, double taxes);
    void HandleNotificationForUnsuccessfulSeller(Auction auction, User user);
    Task HandleNotificationForLoser(Auction auction, Bid bid);
    Task HandleNotificationForAppOwner(Auction auction, double taxes);
}