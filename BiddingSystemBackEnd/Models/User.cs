using System.ComponentModel.DataAnnotations;
using BiddingSystem.Models.Enums;

namespace BiddingSystem.Models;

public class User
{
    public int Id { get; set; }
    [Required] public string UserName { get; set; }
    [Required] public string PasswordHash { get; set; }
    [Required] public string Email { get; set; }
    [Required] public List<Bid> Bids { get; set; } = new();
    [Required] public List<Item> Items { get; set; } = new();
    [Required] public List<Notification> Notifications { get; set; } = new();
    public double Credit { get; set; }
    public double FrozenCredit { get; set; }

    public void AddItem(Item item) => Items.Add(item);

    private void RemoveSoldItem(Item item) => Items.Remove(item);

    private void GetPaid(double amount) => Credit += amount;

    public void SellItem(Auction auction)
    {
        GetPaid(auction.CurrentPrice);
        RemoveSoldItem(auction.Item);
    }

    public void ReceiveNotification(Notification notification)
        => Notifications.Add(notification);

    public void AddBid(Bid bid)
        => Bids.Add(bid);

    public void CancelBid(Bid bid)
    {
        bid.Status = BidStatus.Cancelled;
        UnfreezeCredit(bid.Amount);
    }

    public void CheckIfHasEnoughCredit(double amount)
    {
        if (Credit < amount)
            throw new AggregateException("Not enough credit.");
    }

    public void AddCredit(double amount)
        => Credit += amount;

    public void Pay(double amount)
        => FrozenCredit -= amount;

    public void FreezeCredit(double amount)
    {
        FrozenCredit += amount;
        Credit -= amount;
    }

    private void UnfreezeCredit(double amount)
    {
        FrozenCredit -= amount;
        Credit += amount;
    }

    public void LoseBid(Bid bid)
    {
        bid.Status = BidStatus.Loss;
        UnfreezeCredit(bid.Amount);
    }
}