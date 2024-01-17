using System.IdentityModel.Tokens.Jwt;
using BiddingSystem.Models;

namespace BiddingSystem.Services.Interfaces;

public interface IJwtService
{
    string CreateToken(User user);
    JwtSecurityToken Verify(string jwt);
}