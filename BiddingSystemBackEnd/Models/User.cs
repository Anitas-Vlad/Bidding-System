using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models;

public class User
{
    public int Id { get; set; }
    [Required] public string UserName { get; set; }
    [Required] public string PasswordHash { get; set; }
    [Required] public string Email { get; set; }
    [Required] public List<Bidding> Biddings { get; set; } = new();

    public void RemoveBidding(Bidding bidding) 
        => Biddings.Remove(bidding);

    public void AddBidding(Bidding bidding)
        => Biddings.Add(bidding);
}