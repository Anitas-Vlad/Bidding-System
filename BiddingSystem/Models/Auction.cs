using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models;

public class Auction
{
    public int Id { get; set; }
    [Required] public Item Item { get; set; }
    [Required] public DateTime EndOfAuction { get; set; }
    public double CurrentPrice { get; set; }
    public List<Bidding> Biddings { get; set; } = new();
    public int? WinningBiddingId { get; set; }

    public void AddBidding(Bidding bidding)
    {
        Biddings.Add(bidding);
        WinningBiddingId = bidding.Id;
        CurrentPrice = bidding.Amount;
    }

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
}