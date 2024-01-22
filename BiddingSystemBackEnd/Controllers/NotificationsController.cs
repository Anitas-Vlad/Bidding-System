using BiddingSystem.Models;
using BiddingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiddingSystem.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    [Route("/ViewNotifications")]
    public async Task<ActionResult<List<Notification>>> ViewNotifications()
        => await _notificationService.QueryProfileNotifications();
}