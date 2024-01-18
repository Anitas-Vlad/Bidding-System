using BiddingSystem.Models;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiddingSystem.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuctionsController : ControllerBase
{
    private readonly IAuctionService _auctionService;

    public AuctionsController(IAuctionService auctionService)
    {
        _auctionService = auctionService;
    }
    
    [HttpGet]
    [Route("/auction-{auctionId}")]
    public async Task<ActionResult<Auction>> GetAuctionById(int auctionId) 
        => await _auctionService.QueryAuctionById(auctionId);

    [HttpGet]
    public async Task<ActionResult<List<Auction>>> GetAllAuctions()
        => await _auctionService.QueryAllAuctions();

    [HttpPost]
    [Route("/CreateAuction")]
    public async Task<ActionResult<Auction>> CreateAuction(CreateAuctionRequest request)
        => await _auctionService.CreateAuction(request);

    [HttpPost]
    [Route("/PlaceBidding")]
    public async Task<ActionResult<Auction>> PlaceBiddingForAuction(CreateBidRequest request) 
        => await _auctionService.PlaceBid(request, HttpContext.User);

    [HttpPatch]
    [Route("CancelBidding")]
    public async Task<ActionResult<Auction>> CancelBidding(int biddingId)
        => await _auctionService.CancelBid(biddingId);
}