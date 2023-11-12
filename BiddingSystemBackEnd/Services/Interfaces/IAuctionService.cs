using BiddingSystem.Models;
using BiddingSystem.Models.Requests;

namespace BiddingSystem.Services.Interfaces;

public interface IAuctionService
{
    Task<Auction> QueryAuctionById(int auctionId);
    Task<Auction> CreateAuction(CreateAuctionRequest request);
    Task<Auction> PlaceBiddingForAuction(CreateBiddingRequest request);
    Task<List<Auction>> QueryAllAuctions();
    Task<Auction> CancelBidding(int biddingId);
}