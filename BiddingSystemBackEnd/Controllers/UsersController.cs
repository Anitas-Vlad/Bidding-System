using System.Security.Claims;
using BiddingSystem.Models;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiddingSystem.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService) => _usersService = usersService;

    [HttpGet]
    [Route("/Info")]
    public async Task<ActionResult<User>> GetUserById()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("Invalid or missing user ID claim.");
        }

        return await _usersService.QueryUserById(userId);
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllUsers() 
        => await _usersService.QueryAllUsers();

    [HttpPatch]
    [Route("/AddCredit")]
    public async Task<ActionResult<double>> AddCreditToUser(AddCreditRequest request) 
        => await _usersService.AddCredit(request);

    [HttpPost]
    [Route("/AddItem")]
    public async Task<ActionResult<User>> AddItem(CreateItemRequest request)
        => await _usersService.AddItem(request);
}