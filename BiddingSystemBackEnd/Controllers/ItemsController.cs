using BiddingSystem.Models;
using BiddingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiddingSystem.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemsService;

    public ItemsController(IItemService itemService)
    {
        _itemsService = itemService;
    }

    [HttpGet]
    [Route("/item-{itemId}")]
    public async Task<ActionResult<Item>> GetItemById(int itemId)
        => await _itemsService.QueryItemById(itemId);

    [HttpGet]
    public async Task<ActionResult<List<Item>>> GetAllItems()
        => await _itemsService.QueryAllItems();
}