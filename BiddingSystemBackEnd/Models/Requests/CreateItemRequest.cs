using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models.Requests;

public class CreateItemRequest
{
    [Required] public string Name { get; set; }
}