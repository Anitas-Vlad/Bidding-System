using BiddingSystem.Context;
using BiddingSystem.Models;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiddingSystem.Services;

public class BiddingService : IBiddingService
{
    private readonly BiddingSystemContext _context;
    private readonly IUserContextService _userContextService;

    public BiddingService(BiddingSystemContext context, IUserContextService userContextService)
    {
        _context = context;
        _userContextService = userContextService;
    }

    public async Task<Bid> QueryBidById(int bidId)
    {
        var bidding = await _context.Bids.SingleOrDefaultAsync(bidding => bidding.Id == bidId);
        if (bidding == null) throw new ArgumentException("Bidding not found.");
        return bidding;
    }

    public Bid ConstructBid(CreateBidRequest request)
        =>
            new()
            {
                Amount = request.Amount,
                UserId = _userContextService.GetUserId(),
                AuctionId = request.AuctionId
            };
}