using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BiddingSystem.Models;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace BiddingSystem.Services;

public class AuthService : IAuthService
{
    private readonly IUsersService _usersService;
    private readonly IJwtService _jwtService;

    public AuthService(IUsersService usersService, IJwtService jwtService)
    {
        _usersService = usersService;
        _jwtService = jwtService;
    }

    public async Task<string> Login(LoginRequest request)
    {
        var userFromDb = await _usersService.QueryUserByEmail(request.Email);
        
        if (userFromDb == null) 
            throw new ArgumentException("User not found.");
        
        if (!BCrypt.Net.BCrypt.Verify(request.Password, userFromDb.PasswordHash))
            throw new ArgumentException("Wrong password.");

        var token = _jwtService.CreateToken(userFromDb);

        return token;
    }
}