using BiddingSystem.Models;
using BiddingSystem.Models.Requests;

namespace BiddingSystem.Services.Interfaces;

public interface IBiddingService
{
    Task<Bidding> QueryBiddingById(int biddingId);
    Bidding ConstructBidding(CreateBiddingRequest request);
}