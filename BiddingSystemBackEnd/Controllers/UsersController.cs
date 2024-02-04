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
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    [Route("/PersonalAccount")]
    public async Task<ActionResult<User>> GetPersonalAccount() 
        => await _userService.QueryPersonalAccount();

    [HttpGet]
    [Route("/User-{username}")]
    public async Task<ActionResult<UserResponse>> SearchUserProfile(string username)
        => await _userService.QueryUserProfile(username);
    
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllUsers() 
        => await _userService.QueryAllUsers();

    [HttpPatch]
    [Route("/AddCredit")]
    public async Task<ActionResult<double>> AddCreditToUser(AddCreditRequest request) 
        => await _userService.AddCredit(request);

    [HttpPost]
    [Route("/AddItem")]
    public async Task<ActionResult<Item>> AddItem(CreateItemRequest request)
        => await _userService.AddItem(request);
}