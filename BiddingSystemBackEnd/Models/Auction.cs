using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

namespace BiddingSystem.Models;

public class Auction
{
    public int Id { get; set; }
    [Required] public Item Item { get; set; }
    [Required] public DateTime EndOfAuction { get; set; }
    public double CurrentPrice { get; set; }
    [Required] public double MinimumBidIncrement { get; set; }
    public List<Bid> Bids { get; set; } = new();
    public int? WinningBidId { get; set; }

    public void AddBid(Bid bid)
    {
        Bids.Add(bid);
        WinningBidId = bid.Id;
        CurrentPrice = bid.Amount;
    }

    private double MinimumBidAmount()
    {
        if (!Bids.Any())
            return CurrentPrice;
        
        return CurrentPrice + MinimumBidIncrement;
    }

    public bool IsBidAmountValid(double amount) =>
        amount >= MinimumBidAmount();
    
    public void RemoveBid(Bid bid)
    {
        Bids.Remove(bid);
        SetNewHighestBid();
    }

    private void SetNewHighestBid()
    {
        var secondHighestBid = Bids.MaxBy(bid => bid.Amount);
        
        if (secondHighestBid == null)
        {
            WinningBidId = null;
            CurrentPrice = Item.StartingPrice;
        }
        else
        {
            WinningBidId = secondHighestBid.Id;
            CurrentPrice = secondHighestBid.Amount;
        }
    }

    public Bid? GetBidByUserId(int userId) 
        => Bids.SingleOrDefault(bid => bid.UserId == userId);
}