using BiddingSystem.Context;
using BiddingSystem.Models;
using BiddingSystem.Models.Requests;
using BiddingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiddingSystem.Services;

public class BiddingService : IBiddingService
{
    private readonly BiddingSystemContext _context;

    public BiddingService(BiddingSystemContext context)
    {
        _context = context;
    }

    public async Task<Bid> QueryBidById(int bidId)
    {
        var bidding = await _context.Bids.SingleOrDefaultAsync(bidding => bidding.Id == bidId);
        if (bidding == null) throw new ArgumentException("Bidding not found.");
        return bidding;
    }

    public Bid ConstructBid(CreateBiddingRequest request)
    {
        var bidding = new Bid
        {
            Amount = request.Amount,
            UserId = request.UserId,
            AuctionId = request.AuctionId
        };

        return bidding;
    }
}