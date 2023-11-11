using BiddingSystem.Models.Requests;

namespace BiddingSystem.Services.Interfaces;

public interface IAuthService
{
    Task<string> Login(LoginRequest request);
}