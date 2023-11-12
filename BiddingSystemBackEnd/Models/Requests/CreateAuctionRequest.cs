using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models.Requests;

public class CreateAuctionRequest
{
    [Required] public int ItemId { get; set; }
    [Required] public DateTime EndOfAuction { get; set; }
    [Required] public double MinimumBidIncrement { get; set; }
}