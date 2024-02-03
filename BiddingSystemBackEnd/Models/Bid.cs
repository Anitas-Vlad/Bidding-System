using System.ComponentModel.DataAnnotations;
using BiddingSystem.Models.Enums;

namespace BiddingSystem.Models;

public class Bid
{
    public int Id { get; set; }
    [Required] public double Amount { get; set; }
    [Required] public int UserId { get; set; }
    [Required] public int AuctionId { get; set; }
    [Required] public BidStatus Status { get; set; } = BidStatus.Winning;

    public void UpdateAmount(double newAmount)
        => Amount = newAmount;

    public double GetDifferenceBetweenOldAndNewAmount(double newAmount)
        => newAmount - Amount;
}