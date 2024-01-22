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

    [HttpPost]
    [Route("/Register")]
    public async Task<ActionResult<User>> Register(RegisterRequest request)
        => await _usersService.CreateUser(request);

    [HttpPost]
    [Route("/Login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var jwt = await _authService.Login(request);
        
        Response.Cookies.Append("jwt", jwt, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        });

        return Ok(new
        {
            message = "success",
            jwt
        });
    }

    [HttpPost]
    [Route("/Logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");

        return Ok(new
        {
            message = "success"
        });
    }
}