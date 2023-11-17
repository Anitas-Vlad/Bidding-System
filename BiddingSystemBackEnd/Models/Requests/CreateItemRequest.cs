using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models.Requests;

public class CreateItemRequest
{
    [Required] public string Name { get; set; }
    [Required] public double Price { get; set; }
    [Required] public int OwnerId { get; set; }
}