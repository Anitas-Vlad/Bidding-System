using BiddingSystem.Models;
using BiddingSystem.Models.Requests;

namespace BiddingSystem.Services.Interfaces;

public interface IUsersService
{
    Task<User> QueryUserById(int userId);
    Task<UserDto> QueryUserProfile(string username);
    Task<User> QueryPersonalAccount();
    Task<List<User>> QueryAllUsers();
    Task<User?> QueryUserByEmail(string userEmail);
    Task<User> QueryOwner();
    Task<User> CreateUser(RegisterRequest request);
    Task<double> AddCredit(AddCreditRequest request);
    Task<User> AddItem(CreateItemRequest request);
    void CheckIfUserOwnsBid(Bid bid);
    Task HandleLosingBids(Auction auction);
}