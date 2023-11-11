using BiddingSystem.Context;
using BiddingSystem.Models;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiddingSystem.Services;

public class ItemService : IItemService
{
    private readonly BiddingSystemContext _context;

    public ItemService(BiddingSystemContext context)
    {
        _context = context;
    }

    public async Task<Item> QueryItemById(int itemId)
    {
        var item = await _context.Items.Where(item => item.Id == itemId).SingleOrDefaultAsync();
        
        if (item == null) throw new ArgumentException("Item not Found.");
        
        return item;
    }

    public async Task<List<Item>> QueryAllItems()
        => await _context.Items.ToListAsync();

    public async Task<Item> CreateItem(CreateItemRequest request)
    {
        if (request.Price < 0) throw new ArgumentException("Price cannot be a negative number.");

        var item = new Item()
        {
            Name = request.Name,
            StartingPrice = request.Price
        };

        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        return item;
    }
}