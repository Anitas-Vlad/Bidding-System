using System.ComponentModel.DataAnnotations;

namespace BiddingSystem.Models;

public class Notification
{
    public int Id { get; set; }
    [Required] public int UserId { get; set; }
    [Required] public string Title { get; set; }
    [Required] public string Description { get; set; }
    [Required] public bool IsSeen { get; set; }
}