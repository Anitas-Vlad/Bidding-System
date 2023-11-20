using BiddingSystem.Context;
using BiddingSystem.Models;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiddingSystem.Services;

public class NotificationService : INotificationService
{
    private readonly BiddingSystemContext _context;

    public NotificationService(BiddingSystemContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> QueryAllNotifications()
        => await _context.Notifications.ToListAsync();
    
    public async Task<List<Notification>> QueryNotificationsByUserId(int userId)
        => await _context.Notifications.Where(notification => notification.UserId == userId)
            .OrderBy(notification => notification.IsSeen).ToListAsync();

    public Notification CreateNotification(CreateNotificationRequest request)
    {
        var notification = new Notification
        {
            UserId = request.UserId,
            Title = request.Title,
            Description = request.Description
        };
        return notification;
    }
}