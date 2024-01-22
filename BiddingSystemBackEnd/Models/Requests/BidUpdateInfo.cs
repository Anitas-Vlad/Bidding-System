namespace BiddingSystem.Models.Requests;

public class BidUpdateInfo
{
    public Auction Auction { get; set; }
    public User User { get; set; }
    public Bid PreviouslyWinningBid { get; set; }
    public Bid OptionalPreviousUserBid { get; set; }
    public CreateBidRequest Request { get; set; }
    public Notification? Notification { get; set; }
}