namespace BiddingSystem.Models.Requests;

public class AddCreditRequest
{
    public int UserId { get; set; }
    public double Amount { get; set; }
}