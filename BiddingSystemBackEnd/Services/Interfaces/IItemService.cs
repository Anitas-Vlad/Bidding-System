using BiddingSystem.Models;
using BiddingSystem.Models.Requests;

namespace BiddingSystem.Services.Interfaces;

public interface IItemService
{
    public Task<Item> QueryItemById(int itemId);
    Task<List<Item>> QueryAllItems();
    Item CreateItem(CreateItemRequest request, int userId);
    Task<List<Item>> QueryItemsByUserId(int ownerId);
}