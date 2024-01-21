using System.Security.Claims;
using System.Text.RegularExpressions;
using BiddingSystem.Context;
using BiddingSystem.Models;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiddingSystem.Services;

public class UsersService : IUsersService
{
    private readonly BiddingSystemContext _context;
    private static Regex _mailPattern;
    private static Regex _passwordPattern;
    private readonly IItemService _itemService;
    private readonly IJwtService _jwtService;

    public UsersService(BiddingSystemContext context, IItemService itemService, IJwtService jwtService)
    {
        _context = context;
        _itemService = itemService;
        _mailPattern = new("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
        _passwordPattern = new("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$");
        _jwtService = jwtService;
    }

    public async Task<User> QueryUserById(int userId)
    {
        // var userId = _jwtService.GetUserIdFromClaims(userClaims);
        
        var user = await _context.Users
            .Include(user => user.Bids)
            .Include(user => user.Items)
            .Where(user => user.Id == userId)
            .FirstOrDefaultAsync();

        if (user == null) throw new ArgumentException("User not found.");

        return user;
    }

    public async Task<List<User>> QueryAllUsers() =>
        await _context.Users
            .Include(user => user.Bids)
            .Include(user => user.Items)
            .ToListAsync();

    public async Task<User> QueryUserByEmail(string userEmail)
        => await _context.Users
            .Include(user => user.Bids)
            .Include(user => user.Items)
            .Where(user => user.Email == userEmail)
            .FirstOrDefaultAsync();
    
    public async Task<User> CreateUser(RegisterRequest request)
    {
        await IsEmailValid(request.Email);
        IsPasswordValid(request.Password);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            UserName = request.UserName,
            PasswordHash = passwordHash,
            Email = request.Email
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> AddItem(CreateItemRequest request)
    {
        var user = await QueryUserById(request.UserId);
        var item = _itemService.CreateItem(request);

        user.Items.Add(item);
        _context.Users.Update(user);

        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<double> AddCredit(AddCreditRequest request)
    {
        var user = await QueryUserById(request.UserId);
        user.AddCredit(request.Amount);
        _context.Update(user);
        await _context.SaveChangesAsync();
        return user.Credit;
    } 

    private async Task IsEmailValid(string userEmail)
    {
        if (!_mailPattern.IsMatch(userEmail))
            throw new ArgumentException("Please enter a valid email.");

        if (await QueryUserByEmail(userEmail) != null)
            throw new ArgumentException("Email in use.");
    }

    private static void IsPasswordValid(string userPassword)
    {
        if (!_passwordPattern.IsMatch(userPassword))
            throw new ArgumentException(
                "Password contain special characters, numbers, capital letters and be longer than 8 characters");
    }
}