using BiddingSystem.Context;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace BiddingSystemTests;

public class ItemServiceTests
{
    private readonly DbContextOptions<BiddingSystemContext> _options;
    private readonly BiddingSystemContext _context;
    private readonly DataSeeder _dataSeeder = new();

    private readonly ItemService _itemService;
    
    public ItemServiceTests()
    {
        _options = new DbContextOptionsBuilder<BiddingSystemContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new BiddingSystemContext(_options);

        _dataSeeder.SeedInMemoryDatabase(_options);
        
        _itemService = new ItemService(_context);
    }

    [Fact]
    public async void ShouldReturn_Item_ById()
    {
        var itemId = 1000;

        var item = await _itemService.QueryItemById(itemId);
        
        Assert.NotNull(item);
        Assert.Equal("Napoleon's Favorite Hat", item.Name);
    }
    
    [Fact]
    public async void ShouldThrow_ItemNotFound_ById()
    {
        var itemId = 853273;

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _itemService.QueryItemById(itemId));

        Assert.Equal("Item not found.", exception.Message);
    }

    [Fact]
    public async void ShouldReturn_Items()
    {
        var items = await _itemService.QueryAllItems();

        Assert.NotNull(items);
        Assert.Equal(6, items.Count);
    }

    [Fact]
    public async void ShouldCreate_Item()
    {
        var items = await _itemService.QueryAllItems();
        var request = new CreateItemRequest
        {
            Name = "Test name"
        };
        var userId = 10000;

        var item = _itemService.CreateItem(request, userId);
        
        Assert.NotNull(item);
    }
}