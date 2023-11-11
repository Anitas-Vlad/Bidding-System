using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models;

public class Bidding
{
    public int Id { get; set; }
    [Required] public double Amount { get; set; }
    [Required] public int UserId { get; set; }
    [Required] public int AuctionId { get; set; }
}