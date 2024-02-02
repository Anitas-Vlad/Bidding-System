using BiddingSystem.Models;

namespace BiddingSystem.Services.Interfaces;

public interface IUserMapper
{
    UserResponse Map(User user);
    List<UserResponse> Map(List<User> users);
}