using BiddingSystem.Context;
using BiddingSystem.Models;
using BiddingSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BiddingSystemTests;

public class DataSeeder
{
    public async void SeedInMemoryDatabase(DbContextOptions<BiddingSystemContext> options)
    {
        var context = new BiddingSystemContext(options);

        // Seed specific data for testing
        context.Users.AddRange(
            new User
            {
                Id = 1000,
                Username = "Seller 1",
                Email = "seller1@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Seller1."),
                Credit = 500
            },
            new User
            {
                Id = 1001,
                Username = "Buyer 1",
                Email = "buyer1@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Buyer11."),
                Credit = 5000,
                FrozenCredit = 220
            },
            new User
            {
                Id = 1002,
                Username = "Buyer 2",
                Email = "buyer2@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Buyer22."),
                Credit = 10000,
                FrozenCredit = 220
            },
            new User
            {
                Id = 1003,
                Username = "Buyer 3",
                Email = "buyer3@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Buyer33."),
                Credit = 1500,
                FrozenCredit = 170
            },
            new User
            {
                Id = 1004,
                Username = "Owner",
                Email = "owner@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Owner123."),
                Credit = 0
            }
            // Add more users as needed
        );

        context.Items.AddRange(
            new Item
            {
                Id = 1000,
                UserId = 1000,
                Name = "Napoleon's Favorite Hat",
                AvailableForAuction = false
            },
            new Item
            {
                Id = 1001,
                UserId = 1000,
                Name = "McDonald's Forever Free Nuggets",
                AvailableForAuction = false
            },
            new Item
            {
                Id = 1002,
                UserId = 1000,
                Name = "Eiffel Tower Top Light",
                AvailableForAuction = false
            },
            new Item
            {
                Id = 1003,
                UserId = 1000,
                Name = "Test Item 1",
                AvailableForAuction = true
            },
            new Item
            {
                Id = 1004,
                UserId = 1000,
                Name = "Test Item 2",
                AvailableForAuction = true
            },
            new Item
            {
                Id = 1005,
                UserId = 1000,
                Name = "Test Item 3",
                AvailableForAuction = true
            }
            // Add more items as needed
        );
        // context.Auctions.AddRange(
        //     new Auction
        //     {
        //         Id = 1000,
        //         ItemId = 1000,
        //         SellerId = 1000, // Seller 1
        //         EndOfAuction = DateTime.UtcNow.AddDays(100),
        //         StartingPrice = 100,
        //         CurrentPrice = 100,
        //         MinimumBidIncrement = 10,
        //         WinningBidId = 1000 // Bid Buyer 1
        //     },
        //     new Auction
        //     {
        //         Id = 1001,
        //         ItemId = 1001,
        //         SellerId = 1000, // Seller 1
        //         EndOfAuction = DateTime.UtcNow.AddDays(90),
        //         StartingPrice = 150,
        //         CurrentPrice = 150,
        //         MinimumBidIncrement = 15,
        //         WinningBidId = 1001 // Bid Buyer 3
        //     },
        //     new Auction
        //     {
        //         Id = 1002,
        //         ItemId = 1002,
        //         SellerId = 1000, // Seller 1
        //         EndOfAuction = DateTime.UtcNow.AddDays(100),
        //         StartingPrice = 200,
        //         CurrentPrice = 200,
        //         MinimumBidIncrement = 20,
        //         WinningBidId = 1002 // Bid Buyer 1
        //     },
        //     new Auction
        //     {
        //         Id = 1003,
        //         ItemId = 1003,
        //         SellerId = 1000, // Seller 1
        //         EndOfAuction = DateTime.UtcNow.AddDays(100),
        //         StartingPrice = 200,
        //         CurrentPrice = 200,
        //         MinimumBidIncrement = 20,
        //         WinningBidId = 1002 // Bid Buyer 1
        //     }
        // );
        context.Bids.AddRange(
            new Bid
            {
                Id = 1000,
                Amount = 120,
                UserId = 1001, // Buyer 1
                AuctionId = 1000,
                Status = BidStatus.Winning
            },
            new Bid
            {
                Id = 1001,
                Amount = 170,
                UserId = 1003, // Buyer 3
                AuctionId = 1001,
                Status = BidStatus.Winning
            },
            new Bid
            {
                Id = 1002,
                Amount = 220,
                UserId = 1002, // Buyer 2
                AuctionId = 1002,
                Status = BidStatus.Winning
            },
            new Bid
            {
                Id = 1003,
                Amount = 100,
                UserId = 1001, // Buyer 1
                AuctionId = 1002,
                Status = BidStatus.Winning
            }
        );

        await context.SaveChangesAsync();
    }
}