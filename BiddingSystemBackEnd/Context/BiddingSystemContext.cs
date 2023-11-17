using BiddingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BiddingSystem.Context;

public class BiddingSystemContext : DbContext
{
    public BiddingSystemContext(DbContextOptions<BiddingSystemContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderExtensions.Seed(modelBuilder);
    }

    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Auction> Auctions { get; set; } = default!;
    public DbSet<Item> Items { get; set; } = default!;
    public DbSet<Bid?> Bids { get; set; } = default!;

    private static class ModelBuilderExtensions
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1000,
                    UserName = "AAAAA",
                    Email = "a@a.a",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Aaaaa123."),
                },
                new User
                {
                    Id = 1001,
                    UserName = "BBBBB",
                    Email = "b@b.b",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Bbbbb123."),
                },
                new User
                {
                    Id = 1002,
                    UserName = "CCCCC",
                    Email = "c@c.c",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Ccccc123."),
                }
            );

            modelBuilder.Entity<Item>().HasData(
            
                new Item
                {
                    Id = 1000,
                    Name = "Napoleon's Favorite Hat",
                    StartingPrice = 1000,
                    AvailableForAuction = true
                },
                new Item
                {
                    Id = 1001,
                    Name = "McDonald's Forever Free Nuggets",
                    StartingPrice = 230,
                    AvailableForAuction = true
                },
                new Item
                {
                    Id = 1002,
                    Name = "Eiffel Tower Top Light",
                    StartingPrice = 56000,
                    AvailableForAuction = true
                }
            );
        }
    }
}