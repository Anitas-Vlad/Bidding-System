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

    public async Task<Bidding> QueryBiddingById(int biddingId)
    {
        var bidding = await _context.Biddings.SingleOrDefaultAsync(bidding => bidding.Id == biddingId);
        if (bidding == null) throw new ArgumentException("Bidding not found.");
        return bidding;
    }

    public Bidding ConstructBidding(CreateBiddingRequest request)
    {
        var bidding = new Bidding
        {
            Amount = request.Amount,
            UserId = request.UserId,
            AuctionId = request.AuctionId
        };

        return bidding;
    }
}