using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models;

public class User
{
    public int Id { get; set; }
    [Required] public string UserName { get; set; }
    [Required] public string PasswordHash { get; set; }
    [Required] public string Email { get; set; }
    [Required] public List<Bid> Bids { get; set; } = new();
    public double Credit { get; set; } = 0;
    public double FrozenCredit { get; set; } = 0;

    public void RemoveBid(Bid bid)
        => Bids.Remove(bid);

    public void AddBid(Bid bid)
        => Bids.Add(bid);

    public bool HasEnoughCredit(double amount) => Credit > amount;
    public void AddCredit(double amount) => Credit += amount;

    public void FreezeCredit(double amount)
    {
        FrozenCredit += amount;
        Credit -= amount;
    }
    
    public void UnfreezeCredit(double amount)
    {
        FrozenCredit -= amount;
        Credit += amount;
    }
}