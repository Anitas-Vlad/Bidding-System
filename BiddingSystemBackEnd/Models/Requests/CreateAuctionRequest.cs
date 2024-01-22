using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models.Requests;

public class CreateAuctionRequest
{
    [Required] public int ItemId { get; set; }

    // [Required] public DateTime EndOfAuction { get; set; }
    [Required] public double MinimumBidIncrement { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "The price cannot be a negative number.")]
    public double StartingPrice { get; set; }
}