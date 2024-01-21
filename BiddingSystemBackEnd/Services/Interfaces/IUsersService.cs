using BiddingSystem.Models;
using BiddingSystem.Models.Requests;

namespace BiddingSystem.Services.Interfaces;

public interface IUsersService
{
    Task<User> QueryUserById(int userId);
    Task<User> QueryProfileAccount();
    Task<List<User>> QueryAllUsers();
    Task<User> QueryUserByEmail(string userEmail);
    Task<User> CreateUser(RegisterRequest request);
    Task<double> AddCredit(AddCreditRequest request);
    Task<User> AddItem(CreateItemRequest request);
}