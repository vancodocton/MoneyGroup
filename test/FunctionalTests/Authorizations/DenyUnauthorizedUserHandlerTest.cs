using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.WebApi.Authorizations;

using NSubstitute;

namespace MoneyGroup.FunctionalTests.Authorizations;

public class DenyUnauthorizedUserHandlerTest
{
    private readonly IUserService _userService;
    private readonly ClaimsPrincipal _user;
    private readonly DenyUnauthorizedUserHandler _handler;
    private readonly AuthorizationHandlerContext _authContext;

    public DenyUnauthorizedUserHandlerTest()
    {
        var requirement = new DenyUnauthorizedUserRequirement();
        _userService = Substitute.For<IUserService>();
        _user = Substitute.For<ClaimsPrincipal>();
        _authContext = new AuthorizationHandlerContext([requirement], _user, resource: null);
        _handler = new DenyUnauthorizedUserHandler(NullLoggerFactory.Instance.CreateLogger<DenyUnauthorizedUserHandler>(), _userService);

    }

    [Fact]
    public async Task HandleAsync_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        _user.Identity!.IsAuthenticated.Returns(false);

        // Act
        await _handler.HandleAsync(_authContext);

        // Assert
        Assert.False(_authContext.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WhenUserEmailIsNotPresent_ShouldReturnUnauthorized()
    {
        // Arrange
        _user.Identity!.IsAuthenticated.Returns(true);
        _user.FindFirst(ClaimTypes.Email).Returns((Claim?)null);

        // Act
        await _handler.HandleAsync(_authContext);

        // Assert
        Assert.False(_authContext.HasSucceeded);

    }

    [Fact]
    public async Task HandleAsync_WhenUserEmailIsNotValid_ShouldReturnUnauthorized()
    {
        // Arrange
        _user.Identity!.IsAuthenticated.Returns(true);
        _user.FindFirst(ClaimTypes.Email).Returns(EmailClaim);
        _user.FindFirst(JwtRegisteredClaimNames.EmailVerified).Returns((Claim?)null);

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
        _user.Identity!.IsAuthenticated.Returns(true);
        _user.FindFirst(ClaimTypes.Email).Returns(EmailClaim);
        _user.FindFirst(JwtRegisteredClaimNames.EmailVerified).Returns(new Claim(JwtRegisteredClaimNames.EmailVerified, "true"));
        _userService.GetUserByEmailAsync(Arg.Any<string>(), CancellationToken.None).Returns((UserDto?)null);

        // Act
        await _handler.HandleAsync(_authContext);

        // Assert
        Assert.False(_authContext.HasSucceeded);
    }

    [Fact]
    public async Task HandlerAsync_WhenUserFound_ShouldReturnOk()
    {
        // Arrange
        _user.Identity!.IsAuthenticated.Returns(true);
        _user.FindFirst(ClaimTypes.Email).Returns(EmailClaim);
        _user.FindFirst(JwtRegisteredClaimNames.EmailVerified).Returns(new Claim(JwtRegisteredClaimNames.EmailVerified, "true"));
        _userService.GetUserByEmailAsync(Arg.Any<string>(), CancellationToken.None).Returns(new UserDto()
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
