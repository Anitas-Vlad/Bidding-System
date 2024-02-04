using System.Security.Claims;
using System.Text.RegularExpressions;
using BiddingSystem.Context;
using BiddingSystem.Models;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services;
using BiddingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BiddingSystemTests;

public class UserServiceTests
{
    private readonly DbContextOptions<BiddingSystemContext> _options;
    private readonly BiddingSystemContext _context;

    private readonly UserService _userService;

    private static Regex _mailPattern;
    private static Regex _passwordPattern;
    private readonly IItemService _mockItemService;
    private readonly IUserContextService _mockUserContextService;
    private readonly IUserMapper _mockUserMapper;
    private readonly DataSeeder _dataSeeder = new();

    public UserServiceTests()
    {
        _options = new DbContextOptionsBuilder<BiddingSystemContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new BiddingSystemContext(_options);

        // Seed the in-memory database with test data
        // Assuming _dataSeeder is an instance of a class responsible for seeding data
        _dataSeeder.SeedInMemoryDatabase(_options);

        // Mock dependencies required for UserService
        var mockItemService = new Mock<IItemService>();
        mockItemService.Setup(service => service.CreateItem(It.IsAny<CreateItemRequest>(), 1001))
            .Returns((CreateItemRequest request, int userId) => new Item
            {
                AvailableForAuction = true,
                Id = 2000, // Set the ID as needed
                Name = request.Name, // Use the name from the request
                UserId = userId // Set the user ID
                // You may need to set other properties based on your actual implementation
            });

        _mockUserMapper = new Mock<IUserMapper>().Object;

        // Mock IUserContextService to return a user ID
        var mockUserContextService = new Mock<IUserContextService>();
        mockUserContextService.Setup(service => service.GetUserId()).Returns(1001); // Simulate user ID retrieval

        // Mock IHttpContextAccessor to return a user ID claim
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        // Create a ClaimsIdentity with the desired claim
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1001")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");

        // Create a ClaimsPrincipal and attach the ClaimsIdentity
        var principal = new ClaimsPrincipal(identity);

        // Set up the HttpContext.User property to return the ClaimsPrincipal
        mockHttpContextAccessor.SetupGet(accessor => accessor.HttpContext.User).Returns(principal);

        // Create the instance of UserService with mocked dependencies
        _userService = new UserService(
            _context,
            mockItemService.Object,
            mockUserContextService.Object, // Use the mocked IUserContextService
            _mockUserMapper
        );
    }

    [Fact]
    public async void ShouldReturn_User_ById()
    {
        var userId = 1000;

        var user = await _userService.QueryUserById(userId);

        Assert.NotNull(user);
        Assert.Equal("Seller 1", user.Username);
    }

    [Fact]
    public async void ShouldThrow_UserNotFound_ById()
    {
        var userId = 853273;

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _userService.QueryUserById(userId));

        Assert.Equal("User not found.", exception.Message);
    }

    [Fact]
    public async void ShouldReturn_Users()
    {
        var users = await _userService.QueryAllUsers();

        Assert.NotNull(users);
        Assert.NotEmpty(users);
    }

    [Fact]
    public async void ShouldReturn_User_ByEmail()
    {
        var userEmail = "buyer1@gmail.com";

        var user = await _userService.QueryUserByEmail(userEmail);

        Assert.NotNull(user);
        Assert.Equal("Buyer 1", user.Username);
    }

    [Fact]
    public async void ShouldReturn_Owner()
    {
        var owner = await _userService.QueryOwner();

        Assert.NotNull(owner);
        Assert.Equal("Owner", owner.Username);
    }

    // [Fact]
    // public async void ShouldThrow_InvalidEmail()
    // {
    //     var userEmail = "askdnfs";
    //
    //     var exception = await Assert.ThrowsAsync<ArgumentException>(() => _usersService.IsEmailValid(userEmail));
    //
    //     Assert.Equal("Please enter a valid email.", exception.Message);
    // }
    //
    // [Fact]
    // public async void ShouldThrow_EmailInUse()
    // {
    //     var userEmail = "seller1@gmail.com";
    //     
    //     var exception = await Assert.ThrowsAsync<ArgumentException>(() => _usersService.IsEmailValid(userEmail));
    //     
    //     Assert.Equal($"The email: \"{userEmail}\" in use.", exception.Message);
    // }
    //
    // [Fact]
    // public async void ShouldThrow_UsernameTaken()
    // {
    //     var username = "Seller 1";
    //     
    //     var exception = await Assert.ThrowsAsync<ArgumentException>(() => _usersService.IsUsernameValid(username));
    //     
    //     Assert.Equal($"the username \"{username}\" is taken.", exception.Message);
    // }

    [Fact]
    public async void ShouldReturn_PersonalAccount()
    {
        var user = await _userService.QueryPersonalAccount();

        Assert.NotNull(user);
        Assert.Equal("Buyer 1", user.Username);
    }

    // [Fact]
    public async void ShouldThrow_UserNotFound()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _userService.QueryPersonalAccount());

        Assert.Equal("User not found.", exception.Message);
    }

    [Fact]
    public async void ShouldAddItemToUser()
    {
        var request = new CreateItemRequest { Name = "Test item" };
        var user = await _userService.QueryPersonalAccount();
        var items = _context.Items;

        var item = await _userService.AddItem(request);

        Assert.NotNull(item);
        // Assert.Equal(7, items.Count());
        Assert.Single(user.Items);
    }
}