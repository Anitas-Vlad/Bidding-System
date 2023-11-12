using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models;

public class Auction
{
    public int Id { get; set; }
    [Required] public Item Item { get; set; }
    [Required] public DateTime EndOfAuction { get; set; }
    public double CurrentPrice { get; set; }
    [Required] public double MinimumBidIncrement { get; set; }
    public List<Bidding> Biddings { get; set; } = new();
    public int? WinningBiddingId { get; set; }

    public void AddBidding(Bidding bidding)
    {
        Biddings.Add(bidding);
        WinningBiddingId = bidding.Id;
        CurrentPrice = bidding.Amount;
    }

    private double MinimumBiddingAmount() => CurrentPrice + MinimumBidIncrement;
    
    public bool IsBiddingAmountValid(double amount) =>
        amount > MinimumBiddingAmount();
    
    public void RemoveBidding(Bidding bidding)
    {
        Biddings.Remove(bidding);
        SetNewHighestBidding();
    }

    private void SetNewHighestBidding()
    {
        var secondHighestBidding = Biddings.MaxBy(bidding => bidding.Amount);
        
        if (secondHighestBidding == null)
        {
            WinningBiddingId = null;
            CurrentPrice = Item.StartingPrice;
        }
        else
        {
            WinningBiddingId = secondHighestBidding.Id;
            CurrentPrice = secondHighestBidding.Amount;
        }
    }

    public Bidding? GetBiddingByUserId(int userId) 
        => Biddings.SingleOrDefault(bidding => bidding.UserId == userId);
}