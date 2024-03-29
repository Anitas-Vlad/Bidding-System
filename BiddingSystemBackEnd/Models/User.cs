﻿using System.ComponentModel.DataAnnotations;
using BiddingSystem.Models.Enums;

namespace BiddingSystem.Models;

public class User
{
    public int Id { get; set; }
    [Required] public string Username { get; set; }
    [Required] public string PasswordHash { get; set; }
    [Required] public string Email { get; set; }
    [Required] public List<Bid> Bids { get; set; } = new();
    [Required] public List<Item> Items { get; set; } = new();
    [Required] public List<Notification> Notifications { get; set; } = new();
    [Required] public List<Auction> Auctions { get; set; } = new();
    public double Credit { get; set; }
    public double FrozenCredit { get; set; }

    public void AddItem(Item item)
        => Items.Add(item);

    public void AddAuction(Auction auction)
        => Auctions.Add(auction);

    private void RemoveSoldItem(Item item)
        => Items.Remove(item);

    private void GetPaid(double amount)
        => Credit += amount;

    //TODO maybe not needed.
    public Auction GetAuctionById(int auctionId)
    {
        var auction = Auctions.Find(auction => auction.Id == auctionId);
        if (auction == null)
            throw new ArgumentException("Auction not found.");
        return auction;
    }

    public void Sell(Auction auction, double taxes)
    {
        GetPaid(auction.CurrentPrice);
        RemoveSoldItem(auction.Item);
        Pay(taxes);
    }

    private void Pay(double amount)
        => Credit -= amount;

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

    public void PayWithFrozenCredit(double amount)
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