using BiddingSystem.Models;
using BiddingSystem.Models.Requests;

namespace BiddingSystem.Services.Interfaces;

public interface IBiddingService
{
    Task<Bidding> QueryBiddingById(int biddingId);
    Task<Bidding> CreateBidding(CreateBiddingRequest request);
}