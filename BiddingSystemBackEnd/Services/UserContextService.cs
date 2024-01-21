using System.Security.Claims;
using BiddingSystem.Models;
using BiddingSystem.Services.Interfaces;

namespace BiddingSystem.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new InvalidOperationException("Invalid or missing user ID claim.");
        }

        return userId;
    }
}