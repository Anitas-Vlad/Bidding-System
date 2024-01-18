using System.Drawing;
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
    private readonly ILogger<JwtService> _logger;

    public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("JwtSettings:Token").Value!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var header = new JwtHeader(credentials);

        var payload = new JwtPayload(user.Id.ToString(), null, claims, null, DateTime.Today.AddDays(7));

        var token = new JwtSecurityToken(header, payload);

        var tokenClaims = token.Claims;

        foreach (var claim in tokenClaims)
        {
            Console.WriteLine($"{claim.Type}: {claim.Value}");
        }
      
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
    
    public int GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            // Log or handle the error in a meaningful way
            throw new ArgumentException("Invalid or missing UserId claim in JWT token.");
        }
        
        if (!int.TryParse(userIdClaim.Value, out var userId))
        {
            // Log or handle the error in a meaningful way
            throw new ArgumentException("Invalid or missing UserId claim in JWT token.");
        }

        return userId;
    }
}