using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.WebApi.Authorizations;

using Moq;

namespace MoneyGroup.FunctionalTests.Authorizations;

public class DenyUnauthoredUserHandlerTest
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ClaimsPrincipal> _userMock;
    private readonly DenyUnauthorizedUserHandler _handler;
    private readonly AuthorizationHandlerContext _authContext;

    public DenyUnauthoredUserHandlerTest()
    {
        var requirement = new DenyUnauthorizedUserRequirement();
        _userServiceMock = new();
        _userMock = new();
        _authContext = new AuthorizationHandlerContext([requirement], _userMock.Object, resource: null);
        _handler = new DenyUnauthorizedUserHandler(NullLoggerFactory.Instance.CreateLogger<DenyUnauthorizedUserHandler>(), _userServiceMock.Object);

    }

    [Fact]
    public async Task HandleAsync_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        _userMock.Setup(u => u.Identity!.IsAuthenticated).Returns(false);

        // Act
        await _handler.HandleAsync(_authContext);

        // Assert
        Assert.False(_authContext.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WhenUserEmailIsNotPresent_ShouldReturnUnauthorized()
    {
        // Arrange
        _userMock.Setup(u => u.Identity!.IsAuthenticated).Returns(true);
        _userMock.Setup(u => u.FindFirst(ClaimTypes.Email)).Returns((Claim?)null);

        // Act
        await _handler.HandleAsync(_authContext);

        // Assert
        Assert.False(_authContext.HasSucceeded);

    }

    [Fact]
    public async Task HandleAsync_WhenUserEmailIsNotValid_ShouldReturnUnauthorized()
    {
        // Arrange
        _userMock.Setup(u => u.Identity!.IsAuthenticated).Returns(true);
        _userMock.Setup(u => u.FindFirst(ClaimTypes.Email)).Returns(EmailClaim);
        _userMock.Setup(u => u.FindFirst(JwtRegisteredClaimNames.EmailVerified)).Returns((Claim?)null);

        // Act
        await _handler.HandleAsync(_authContext);

        // Assert
        Assert.False(_authContext.HasSucceeded);
    }

    private const string Email = "user@domain.com";
    private static readonly Claim EmailClaim = new(ClaimTypes.Email, Email);

    [Fact]
    public async Task HandlerAsync_WhenUserNotFound_ShouldReturnUnauthorized()
    {
        // Arrange
        _userMock.Setup(u => u.Identity!.IsAuthenticated).Returns(true);
        _userMock.Setup(u => u.FindFirst(ClaimTypes.Email)).Returns(EmailClaim);
        _userMock.Setup(u => u.FindFirst(JwtRegisteredClaimNames.EmailVerified)).Returns(new Claim(JwtRegisteredClaimNames.EmailVerified, "true"));
        _userServiceMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync((UserDto?)null);

        // Act
        await _handler.HandleAsync(_authContext);

        // Assert
        Assert.False(_authContext.HasSucceeded);
    }

    [Fact]
    public async Task HandlerAsync_WhenUserFound_ShouldReturnOk()
    {
        // Arrange
        _userMock.Setup(u => u.Identity!.IsAuthenticated).Returns(true);
        _userMock.Setup(u => u.FindFirst(ClaimTypes.Email)).Returns(EmailClaim);
        _userMock.Setup(u => u.FindFirst(JwtRegisteredClaimNames.EmailVerified)).Returns(new Claim(JwtRegisteredClaimNames.EmailVerified, "true"));
        _userServiceMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(new UserDto()
        {
            Id = 1,
            Name = "User",
            Email = Email,
        });

        // Act
        await _handler.HandleAsync(_authContext);

        // Assert
        Assert.True(_authContext.HasSucceeded);
    }
}
