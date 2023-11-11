using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models;

public class Item
{
    public int Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public double StartingPrice { get; set; }
    [Required] public bool AvailableForAuction { get; set; } = true;
}