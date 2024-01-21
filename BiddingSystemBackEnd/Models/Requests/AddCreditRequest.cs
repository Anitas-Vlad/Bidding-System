using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models.Requests;

public class AddCreditRequest
{
    
    [Range(0, double.MaxValue, ErrorMessage = "Positive amount required.")] 
    public double Amount { get; set; }
}