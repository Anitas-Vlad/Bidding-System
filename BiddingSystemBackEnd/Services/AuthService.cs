using BiddingSystem.Models.Requests;
using BiddingSystem.Services.Interfaces;

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
        
        if (userFromDb == null || !BCrypt.Net.BCrypt.Verify(request.Password, userFromDb.PasswordHash)) 
            throw new ArgumentException("Incorrect credentials.");

        var token = _jwtService.CreateToken(userFromDb);

        return token;
    }
}