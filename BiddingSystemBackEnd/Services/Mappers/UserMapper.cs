using BiddingSystem.Models;
using BiddingSystem.Services.Interfaces;

namespace BiddingSystem.Services.Mappers;

public class UserMapper : IUserMapper
{
    public UserResponse Map(User user)
        => new()
        {
            Id = user.Id,
            Bids = user.Bids,
            Items = user.Items,
            UserName = user.Username
        };

    public List<UserResponse> Map(List<User> users) 
        => users.Select(user => Map(user)).ToList();
}