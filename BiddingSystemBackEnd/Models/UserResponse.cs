using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models;

public class UserResponse
{
    public int Id { get; set; }
    [Required] public string UserName { get; set; }
    [Required] public List<Bid> Bids { get; set; } = new();
    [Required] public List<Item> Items { get; set; } = new();
}