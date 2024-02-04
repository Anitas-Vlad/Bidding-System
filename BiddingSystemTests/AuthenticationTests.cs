using BiddingSystem.Models.Requests;
using BiddingSystem.Services;
using BiddingSystem.Services.Interfaces;
using Moq;

namespace BiddingSystemTests;

public class AuthenticationTests
{
    private readonly AuthService _authService;

    private readonly IUserService _mockUserService;
    private readonly IJwtService _mockJwtService;

    public AuthenticationTests()
    {
        _mockUserService = new Mock<IUserService>().Object;
        _mockJwtService = new Mock<IJwtService>().Object;
        _authService = new AuthService(_mockUserService, _mockJwtService);
    }
    
    [Fact]
    public async void ValidLogin_ReturnsAuthToken()
    {
        // Arrange
        var validCredentials = new LoginRequest
        {
            Email = "owner@gmail.com",
            Password = "Owner123."
        };

        // Act
        var authToken = await _authService.Login(validCredentials);

        // Assert
        Assert.NotNull(authToken);
    }
    
    [Fact]
    public async void InvalidLogin_ThrowsUserNotFound()
    {
        // Arrange
        var invalidCredentials = new LoginRequest
        {
            Email = "124@gmail.com",
            Password = "Owner123."
        };

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>
            (() => _authService.Login(invalidCredentials));

        // Assert
        Assert.Equal("User not found.", exception.Message);
    }
    
    [Fact]
    public async void InvalidLogin_ThrowsWrongPassword()
    {
        // Arrange
        var invalidCredentials = new LoginRequest
        {
            Email = "owner@gmail.com",
            Password = "asd."
        };

        // Act and Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>
            (() => _authService.Login(invalidCredentials));

        // Assert
        Assert.Equal("Wrong password.", exception.Message);
    }

    public void Dispose()
    {
        // Dispose of your disposable resource
    }
}