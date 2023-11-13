using BiddingSystem.Models;
using BiddingSystem.Models.Requests;

namespace BiddingSystem.Services.Interfaces;

public interface IBiddingService
{
    Task<Bid> QueryBidById(int bidId);
    Bid ConstructBid(CreateBiddingRequest request);
}