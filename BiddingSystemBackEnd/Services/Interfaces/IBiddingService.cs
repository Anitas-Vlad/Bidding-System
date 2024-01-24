using BiddingSystem.Models;
using BiddingSystem.Models.Requests;

namespace BiddingSystem.Services.Interfaces;

public interface IBiddingService
{
    Task<Bid> QueryBidById(int bidId);
    Task<Bid> CreateBid(CreateBidRequest request);
}