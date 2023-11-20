using BiddingSystem.Models;
using BiddingSystem.Models.Requests;

namespace BiddingSystem.Services.Interfaces;

public interface INotificationService
{
    Task<List<Notification>> QueryAllNotifications();
    Task<List<Notification>> QueryNotificationsByUserId(int userId);
    Notification CreateNotification(CreateNotificationRequest request);
}