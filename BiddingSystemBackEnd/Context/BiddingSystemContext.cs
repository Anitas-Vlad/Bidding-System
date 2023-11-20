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
        // modelBuilder.Entity<Item>()
        //     .HasOne<User>()
        //     .WithMany()
        //     .HasForeignKey(i => i.UserId)
        //     .IsRequired();
        //
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
                    UserName = "AAAAA",
                    Email = "a@a.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Aaaaa123."),
                    Credit = 10000
                },
                new User
                {
                    Id = 1001,
                    UserName = "BBBBB",
                    Email = "b@b.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Bbbbb123."),
                    Credit = 10000
                },
                new User
                {
                    Id = 1002,
                    UserName = "CCCCC",
                    Email = "c@c.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Ccccc123."),
                    Credit = 10000
                }
            );

            modelBuilder.Entity<Item>().HasData(
            
                new Item
                {
                    Id = 1000,
                    UserId = 1000,
                    Name = "Napoleon's Favorite Hat",
                    StartingPrice = 1000,
                    AvailableForAuction = true
                },
                new Item
                {
                    Id = 1001,
                    UserId = 1002,
                    Name = "McDonald's Forever Free Nuggets",
                    StartingPrice = 230,
                    AvailableForAuction = true
                },
                new Item
                {
                    Id = 1002,
                    UserId = 1002,
                    Name = "Eiffel Tower Top Light",
                    StartingPrice = 56000,
                    AvailableForAuction = true
                }
            );
        }
    }
}