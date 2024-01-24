using System.Text.RegularExpressions;
using BiddingSystem.Context;
using BiddingSystem.Models;
using BiddingSystem.Models.Enums;
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
    private readonly IUserContextService _userContextService;
    private readonly INotificationService _notificationService;

    public UsersService(BiddingSystemContext context, IItemService itemService,
        IUserContextService userContextService, INotificationService notificationService)
    {
        _context = context;
        _itemService = itemService;
        _mailPattern = new("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
        _passwordPattern = new("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$");
        _userContextService = userContextService;
        _notificationService = notificationService;
    }

    public async Task<User> QueryUserById(int userId)
    {
        var user = await _context.Users
            .Include(user => user.Bids)
            .Include(user => user.Items)
            .Where(user => user.Id == userId)
            .FirstOrDefaultAsync();

        if (user == null) throw new ArgumentException("User not found.");

        return user;
    }

    public async Task<User> QueryProfileAccount()
    {
        var userId = _userContextService.GetUserId();

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

    public async Task<User?> QueryUserByEmail(string userEmail)
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
        var userId = _userContextService.GetUserId();
        var user = await QueryUserById(userId);
        var item = _itemService.CreateItem(request, userId);

        user.Items.Add(item);
        _context.Users.Update(user);

        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<double> AddCredit(AddCreditRequest request)
    {
        var userId = _userContextService.GetUserId();
        var user = await QueryUserById(userId);

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
                "Password must contain special characters, numbers, capital letters and be longer than 8 characters");
    }

    public void CheckIfUserOwnsBid(Bid bid)
    {
        var userProfileId = _userContextService.GetUserId();

        var isSameUserId = userProfileId == bid.UserId;

        if (!isSameUserId)
            throw new InvalidOperationException("Invalid user ID claim.");
    }

    public async Task HandleLosingBids(Auction auction)
    {
        var losingBids = auction.Bids.Where(bid => bid.Status == BidStatus.Losing);
        foreach (var bid in losingBids)
        {
            bid.Status = BidStatus.Loss;

            var user = await QueryUserById(bid.UserId);
            user.LoseBid(bid);
            
            await _notificationService.HandleNotificationForLoser(auction, bid);

            _context.Users.Update(user);
            _context.Bids.Update(bid);
        }
    }
}