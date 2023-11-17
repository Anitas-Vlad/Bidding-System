using System.Text.Json.Serialization;

namespace BiddingSystem.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AuctionStatus
{
    NoBids,
    InProgress,
    Finished 
}