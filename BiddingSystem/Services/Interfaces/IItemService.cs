using BiddingSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace BiddingSystem.Services.Interfaces;

public interface IItemService
{
    public Task<Item> QueryItemById(int itemId);
    Task<List<Item>> QueryAllItems();
}