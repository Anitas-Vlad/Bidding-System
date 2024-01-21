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

    public Item CreateItem(CreateItemRequest request, int userId)
    {
        var item = new Item
        {
            Name = request.Name,
            UserId = userId
        };

        _context.Items.Add(item);
        
        return item;
    }

    public async Task<List<Item>> QueryItemsByUserId(int ownerId)
        => await _context.Items.Where(item => item.UserId == ownerId).ToListAsync();
}