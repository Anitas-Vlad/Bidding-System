using BiddingSystem.Models;
using BiddingSystem.Services.Interfaces;

namespace BiddingSystem.Services.Mappers;

public class UserMapper : IUserMapper
{
    public UserDto Map(User user)
        => new()
        {
            Id = user.Id,
            Bids = user.Bids,
            Items = user.Items,
            UserName = user.UserName
        };

    public List<UserDto> Map(List<User> users) 
        => users.Select(user => Map(user)).ToList();
}