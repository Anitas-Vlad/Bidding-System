using BiddingSystem.Models;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiddingSystem.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsersService _usersService;
    private readonly IAuthService _authService;

    public AuthController(IUsersService usersService, IAuthService authService)
    {
        _usersService = usersService;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(RegisterRequest request)
        => await _usersService.CreateUser(request);

    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(LoginRequest request)
        => Ok(await _authService.Login(request));
}