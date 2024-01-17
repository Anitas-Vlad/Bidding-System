using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BiddingSystem.Models;
using BiddingSystem.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace BiddingSystem.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("JwtSettings:Token").Value!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var header = new JwtHeader(credentials);

        var payload = new JwtPayload(user.Id.ToString(), null, claims, null, DateTime.Today.AddDays(7));

        var token = new JwtSecurityToken(header, payload);

        // var token = new JwtSecurityToken(
        //     claims: claims,
        //     expires: DateTime.Now.AddDays(1),
        //     signingCredentials: credentials
        // );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public JwtSecurityToken Verify(string jwt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSettings:Token").Value!);
        
        tokenHandler.ValidateToken(jwt, new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false
        }, out var validatedToken);
        
        return (JwtSecurityToken)validatedToken;
    }
}