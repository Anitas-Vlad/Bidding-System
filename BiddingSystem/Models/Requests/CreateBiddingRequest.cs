using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models.Requests;

public class CreateBiddingRequest
{
    [Required] public double Amount { get; set; }
    [Required] public int UserId { get; set; }
    [Required] public int AuctionId { get; set; }
}