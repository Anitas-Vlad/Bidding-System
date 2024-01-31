﻿using System.ComponentModel.DataAnnotations;
using BiddingSystem.Models.Enums;

namespace BiddingSystem.Models;

public class Auction
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    [Required] public Item Item { get; set; }
    [Required] public int SellerId { get; set; }
    [Required] public DateTime EndOfAuction { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "The price cannot be a negative number.")]
    public double StartingPrice { get; set; }

    [Required] public double CurrentPrice { get; set; }
    [Required] public double MinimumBidIncrement { get; set; }
    public List<Bid> Bids { get; set; } = new();

    public int? WinningBidId { get; set; }

    public void AddBid(Bid bid)
    {
        Bids.Add(bid);
        WinningBidId = bid.Id;
        CurrentPrice = bid.Amount;
    }

    public Bid? GetWinningBid() =>
        Bids.SingleOrDefault(bid => bid.Status == BidStatus.Winning)!;

    private double MinimumBidAmount()
    {
        if (!Bids.Any() || Bids.All(bid => bid.Status == BidStatus.Cancelled))
            return CurrentPrice;

        return CurrentPrice + MinimumBidIncrement;
    }

    public void IsBidAmountValid(double amount)
    {
        if (amount < MinimumBidAmount())
            throw new ArgumentException(
                "The bidding amount is not valid. Check if you respect the MinimumBidIncrement");
    }

    public List<Bid> GetLosingBids()
        => Bids.Where(bid => bid.Status == BidStatus.Losing).ToList();

    public string GetItemName()
        => Item.Name;

    public bool CheckIfBidToRemoveIsTheHighest(Bid bid)
        => bid.Id == WinningBidId;

    public void RemoveLosingBid(Bid bid) // TODO check if needed
        => Bids.Remove(bid);

    public Bid? RemoveWinningBid(Bid bid) // TODO check if needed
    {
        // Bids.Remove(bid); //TODO check if needed
        return SetNewHighestBid();
    }

    public double GetBidAmountByUserId(int userId) =>
        Bids.Where(bid => bid.UserId == userId)
            .Select(bid => bid.Amount)
            .FirstOrDefault();

    public Bid? SetNewHighestBid()
    {
        var secondHighestBid = GetLosingBids().
            MaxBy(bid => bid.Amount);

        if (secondHighestBid == null)
        {
            WinningBidId = 0;
            CurrentPrice = StartingPrice;
        }
        else
        {
            secondHighestBid.Status = BidStatus.Winning;
            WinningBidId = secondHighestBid.Id;
            CurrentPrice = secondHighestBid.Amount;
        }

        return secondHighestBid;
    }

    public Bid? GetBidByUserId(int userId)
        => Bids.SingleOrDefault(bid => bid.UserId == userId);
}