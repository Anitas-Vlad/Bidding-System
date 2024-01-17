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
    private readonly IJwtService _jwtService;

    public AuthController(IUsersService usersService, IAuthService authService, IJwtService jwtService)
    {
        _usersService = usersService;
        _authService = authService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(RegisterRequest request)
        => await _usersService.CreateUser(request);

    [HttpPost("login")]
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
    
    [HttpGet("user")]
    public async Task<IActionResult> User()
    {
        try
        {
            var jwt = Request.Cookies["jwt"];
            
            var token = _jwtService.Verify(jwt);
            var userId = int.Parse(token.Issuer);
            var user = await _usersService.QueryUserById(userId);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return Unauthorized();
        }
    }
    
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");

        return Ok(new
        {
            message = "success"
        });
    }
}