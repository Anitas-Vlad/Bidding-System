﻿using System.Text.Json.Serialization;

namespace BiddingSystem.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BidStatus
{
    Winning,
    Losing,
    Win,
    Loss,
    Cancelled
}