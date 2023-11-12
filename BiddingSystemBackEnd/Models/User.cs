using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models;

public class User
{
    public int Id { get; set; }
    [Required] public string UserName { get; set; }
    [Required] public string PasswordHash { get; set; }
    [Required] public string Email { get; set; }
    [Required] public List<Bidding> Biddings { get; set; } = new();
    public double Credit { get; set; } = 0;
    public double FrozenCredit { get; set; } = 0;

    public void RemoveBidding(Bidding bidding)
        => Biddings.Remove(bidding);

    public void AddBidding(Bidding bidding)
        => Biddings.Add(bidding);

    // TODO This will get more complicated when adding the FrozenMoney Implementation
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