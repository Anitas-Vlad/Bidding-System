﻿using BiddingSystem.Models;
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
    [Route("/ProfileAccount")]
    public async Task<ActionResult<User>> GetProfileAccount() 
        => await _usersService.QueryPersonalAccount();

    [HttpGet]
    [Route("/User-{userId}")]
    public async Task<ActionResult<UserDto>> SearchUser(int userId)
        => await _usersService.QueryUserProfile(userId);
    
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