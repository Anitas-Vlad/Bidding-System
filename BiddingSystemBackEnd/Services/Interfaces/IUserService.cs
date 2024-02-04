using BiddingSystem.Models;
using BiddingSystem.Models.Requests;

namespace BiddingSystem.Services.Interfaces;

public interface IUserService
{
    Task<User> QueryUserById(int userId);
    Task<UserResponse> QueryUserProfile(string username);
    Task<User> QueryPersonalAccount();
    Task<List<User>> QueryAllUsers();
    Task<User?> QueryUserByEmail(string userEmail);
    Task<User> QueryOwner();
    Task<User> CreateUser(RegisterRequest request);
    Task<double> AddCredit(AddCreditRequest request);
    Task<Item> AddItem(CreateItemRequest request);
    void CheckIfUserOwnsBid(Bid bid);
}