using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.UnitTests.Builders;
using MoneyGroup.WebApi.Endpoints;
using MoneyGroup.WebApi.Features;

using NSubstitute;

namespace MoneyGroup.UnitTests.WebApi.Endpoints;

[Trait("Category", "Unit")]
public class UserEndpointsTests
{
    private readonly IUserService _userService = Substitute.For<IUserService>();

    [Fact]
    public async Task GivenUserId_WhenUserExists_ThenReturnsOk()
    {
        // Arrange
        var user = UserDtoBuilder.Valid().WithId(1).Build();
        _userService.GetUserByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await UserEndpoints.GetUserByIdAsync(
            1, _userService, TestContext.Current.CancellationToken);

        // Assert
        var ok = Assert.IsType<Ok<UserDto>>(result.Result);
        Assert.Same(user, ok.Value);
    }

    [Fact]
    public async Task GivenUserId_WhenUserMissing_ThenReturnsNotFound()
    {
        // Arrange
        _userService.GetUserByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((UserDto?)null);

        // Act
        var result = await UserEndpoints.GetUserByIdAsync(
            int.MaxValue, _userService, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task GivenPagingRequest_WhenCalled_ThenReturnsOkWithServiceResult()
    {
        // Arrange
        var request = new UserPaginatedRequest(keyword: null, page: 1, size: 10);
        var expected = new PaginatedModel<UserDto> { Page = 1, Count = 0, Total = 0, Items = [] };
        _userService.GetUsersByPageAsync(request, Arg.Any<CancellationToken>()).Returns(expected);

        // Act
        var result = await UserEndpoints.GetUsersAsync(request, _userService);

        // Assert
        var ok = Assert.IsType<Ok<PaginatedModel<UserDto>>>(result.Result);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public void GivenHttpContext_WhenCurrentUserFeaturePresent_ThenReturnsThatUser()
    {
        // Arrange
        var user = UserDtoBuilder.Valid().WithId(5).Build();
        var httpContext = new DefaultHttpContext();
        httpContext.Features.Set<ICurrentUserFeature>(new CurrentUserFeature { User = user });

        // Act
        var result = UserEndpoints.GetExecutingUser(httpContext);

        // Assert
        Assert.Same(user, result.Value);
    }

    [Fact]
    public void GivenHttpContext_WhenCurrentUserFeatureMissing_ThenThrowsArgumentNull()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => UserEndpoints.GetExecutingUser(httpContext));
    }
}
