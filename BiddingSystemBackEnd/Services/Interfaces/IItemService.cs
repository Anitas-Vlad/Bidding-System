using BiddingSystem.Models;
using BiddingSystem.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BiddingSystem.Services.Interfaces;

public interface IItemService
{
    public Task<Item> QueryItemById(int itemId);
    Task<List<Item>> QueryAllItems();
    Item CreateItem(CreateItemRequest request);
    Task<List<Item>> QueryItemsByUserId(int ownerId);
}