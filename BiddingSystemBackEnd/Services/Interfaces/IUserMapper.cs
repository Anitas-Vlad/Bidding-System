using BiddingSystem.Models;

namespace BiddingSystem.Services.Interfaces;

public interface IUserMapper
{
    UserDto Map(User user);
    List<UserDto> Map(List<User> users);
}