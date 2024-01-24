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
        // modelBuilder.Entity<Bid>()
        //     .HasOne(b => b.User)
        //     .WithMany(u => u.Bids)
        //     .HasForeignKey(b => b.UserId)
        //     .OnDelete(DeleteBehavior.Cascade);
        
        ModelBuilderExtensions.Seed(modelBuilder);
    }

    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Auction> Auctions { get; set; } = default!;
    public DbSet<Item> Items { get; set; } = default!;
    public DbSet<Bid> Bids { get; set; } = default!;
    public DbSet<Notification> Notifications { get; set; } = default!;

    private static class ModelBuilderExtensions
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1000,
                    UserName = "Seller 1",
                    Email = "seller1@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Seller1."),
                    Credit = 10000
                },
                new User
                {
                    Id = 1001,
                    UserName = "Buyer 1",
                    Email = "buyer1@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Buyer11."),
                    Credit = 10000
                },
                new User
                {
                    Id = 1002,
                    UserName = "Buyer 2",
                    Email = "buyer2@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Buyer22."),
                    Credit = 10000
                },
                new User
                {
                    Id = 1003,
                    UserName = "Buyer 3",
                    Email = "buyer3@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Buyer33."),
                    Credit = 0
                },
                new User
                {
                    Id = 1004,
                    UserName = "Owner",
                    Email = "owner@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Owner123."),
                    Credit = 0
                }
            );

            modelBuilder.Entity<Item>().HasData(
                new Item
                {
                    Id = 1000,
                    UserId = 1000,
                    Name = "Napoleon's Favorite Hat",
                    AvailableForAuction = true
                },
                new Item
                {
                    Id = 1001,
                    UserId = 1000,
                    Name = "McDonald's Forever Free Nuggets",
                    AvailableForAuction = true
                },
                new Item
                {
                    Id = 1002,
                    UserId = 1002,
                    Name = "Eiffel Tower Top Light",
                    AvailableForAuction = true
                }
            );
        }
    }
}