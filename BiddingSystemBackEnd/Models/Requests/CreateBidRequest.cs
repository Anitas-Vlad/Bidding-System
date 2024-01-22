using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models.Requests;

public class CreateBidRequest
{
    [Required] public double Amount { get; set; }
    [Required] public int AuctionId { get; set; }
}