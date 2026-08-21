using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.JsonWebTokens;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.WebApi.Authorizations;

using NSubstitute;

namespace MoneyGroup.UnitTests.WebApi.Authorizations;

[Trait("Category", "Unit")]
public class DenyUnauthorizedUserHandlerTests
{
    private const string Email = "user@domain.com";

    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly DenyUnauthorizedUserHandler _handler;

    public DenyUnauthorizedUserHandlerTests()
    {
        _handler = new DenyUnauthorizedUserHandler(
            NullLoggerFactory.Instance.CreateLogger<DenyUnauthorizedUserHandler>(),
            _userService);
    }

    private static AuthorizationHandlerContext ContextFor(ClaimsPrincipal user) =>
        new([new DenyUnauthorizedUserRequirement()], user, resource: null);

    private static ClaimsPrincipal Anonymous() => new(new ClaimsIdentity());

    private static ClaimsPrincipal Authenticated(params Claim[] claims) =>
        new(new ClaimsIdentity(claims, authenticationType: "TestAuth"));

    [Fact]
    public async Task GivenUser_WhenNotAuthenticated_ThenDoesNotSucceed()
    {
        // Arrange
        var context = ContextFor(Anonymous());

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task GivenUser_WhenEmailClaimMissing_ThenDoesNotSucceed()
    {
        // Arrange
        var context = ContextFor(Authenticated());

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task GivenUser_WhenEmailVerifiedClaimMissing_ThenDoesNotSucceed()
    {
        // Arrange
        var context = ContextFor(Authenticated(new Claim(ClaimTypes.Email, Email)));

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task GivenVerifiedEmail_WhenUserNotFound_ThenDoesNotSucceed()
    {
        // Arrange
        var context = ContextFor(Authenticated(
            new Claim(ClaimTypes.Email, Email),
            new Claim(JwtRegisteredClaimNames.EmailVerified, "true")));

        _userService.GetUserByEmailAsync(Email, Arg.Any<CancellationToken>())
            .Returns((UserDto?)null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
        await _userService.Received(1).GetUserByEmailAsync(Email, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenVerifiedEmail_WhenUserFound_ThenSucceeds()
    {
        // Arrange
        var context = ContextFor(Authenticated(
            new Claim(ClaimTypes.Email, Email),
            new Claim(JwtRegisteredClaimNames.EmailVerified, "true")));

        _userService.GetUserByEmailAsync(Email, Arg.Any<CancellationToken>())
            .Returns(new UserDto { Id = 1, Name = "User", Email = Email });

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
        await _userService.Received(1).GetUserByEmailAsync(Email, Arg.Any<CancellationToken>());
    }
}
