using System.Security.Claims;
using BiddingSystem.Context;
using BiddingSystem.Services;
using BiddingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BiddingSystemTests;

public class AuctionServiceTests : IDisposable
{
    private readonly DbContextOptions<BiddingSystemContext> _options;
    private readonly BiddingSystemContext _context;
    
    private readonly AuctionService _auctionService;

    private readonly IItemService _mockItemService;
    private readonly IBiddingService _mockBiddingService;
    private readonly IUserService _mockUserService;
    private readonly INotificationService _mockNotificationService;
    private readonly DataSeeder _dataSeeder = new();


    public AuctionServiceTests()
    {
        _options = new DbContextOptionsBuilder<BiddingSystemContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new BiddingSystemContext(_options);
        
        _dataSeeder.SeedInMemoryDatabase(_options);
        
        _mockItemService = new Mock<IItemService>().Object;
        _mockBiddingService = new Mock<IBiddingService>().Object;
        _mockUserService = new Mock<IUserService>().Object;
        _mockNotificationService = new Mock<INotificationService>().Object;

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

        _auctionService = new AuctionService(
            _context,
            _mockItemService,
            _mockBiddingService,
            _mockUserService,
            _mockNotificationService,
            mockUserContextService.Object
        );
    }

    public void Dispose() 
        => _context.Dispose();

    [Fact]
    public async void ShouldReturnAuction()
    {
        //Arrange
        var auctionId = 1000;

        //Act
        var auction = await _auctionService.QueryAuctionById(auctionId);
        // var itemId = auction.ItemId;

        //Assert
        Assert.NotNull(auction);
        Assert.Equal(1000, auctionId);
    }

    [Fact]
    public async void ShouldThrow_AuctionNotFound()
    {
        // Arrange
        var nonExistentAuctionId = 999999; // A non-existent auction ID

        // Act and Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _auctionService.QueryAuctionById(nonExistentAuctionId));
        
        // Assert
        Assert.Equal("Auction not Found.", exception.Message);
    }

    [Fact]
    public async void ShouldReturn_Auctions()
    {
        var auctions = await _auctionService.QueryAllAuctions();
        Assert.NotNull(auctions);
        Assert.NotEmpty(auctions);
    }
}