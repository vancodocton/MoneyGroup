using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.Core.Services;
using MoneyGroup.Core.Specifications;
using MoneyGroup.UnitTests.Builders;

using NSubstitute;

namespace MoneyGroup.UnitTests.Core.Services;

[Trait("Category", "Unit")]
public class UserServiceTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userService = new UserService(_userRepository);
    }

    [Fact]
    public async Task GivenUserId_WhenUserExists_ThenReturnsUser()
    {
        // Arrange
        var user = UserDtoBuilder.Valid().WithId(7).Build();
        var cancellationToken = TestContext.Current.CancellationToken;

        _userRepository.FirstOrDefaultAsync<UserDto>(Arg.Any<EntityByIdSpec<User>>(), cancellationToken)
            .Returns(user);

        // Act
        var result = await _userService.GetUserByIdAsync(7, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Id);
        await _userRepository.Received(1)
            .FirstOrDefaultAsync<UserDto>(Arg.Any<EntityByIdSpec<User>>(), cancellationToken);
    }

    [Fact]
    public async Task GivenUserId_WhenUserMissing_ThenReturnsNull()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        _userRepository.FirstOrDefaultAsync<UserDto>(Arg.Any<EntityByIdSpec<User>>(), cancellationToken)
            .Returns((UserDto?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(int.MaxValue, cancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GivenEmail_WhenUserExists_ThenReturnsUser()
    {
        // Arrange
        var user = UserDtoBuilder.Valid().WithEmail("known@domain.com").Build();
        var cancellationToken = TestContext.Current.CancellationToken;

        _userRepository.FirstOrDefaultAsync<UserDto>(Arg.Any<UserByEmailSpec>(), cancellationToken)
            .Returns(user);

        // Act
        var result = await _userService.GetUserByEmailAsync("known@domain.com", cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("known@domain.com", result.Email);
        await _userRepository.Received(1)
            .FirstOrDefaultAsync<UserDto>(Arg.Any<UserByEmailSpec>(), cancellationToken);
    }

    [Fact]
    public async Task GivenEmail_WhenUserMissing_ThenReturnsNull()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        _userRepository.FirstOrDefaultAsync<UserDto>(Arg.Any<UserByEmailSpec>(), cancellationToken)
            .Returns((UserDto?)null);

        // Act
        var result = await _userService.GetUserByEmailAsync("absent@domain.com", cancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GivenPagingOptions_WhenUsersExist_ThenReturnsPaginatedUsers()
    {
        // Arrange
        var options = new UserPaginatedOptions(page: 1, size: 10);
        var expected = new PaginatedModel<UserDto>
        {
            Page = 1,
            Count = 1,
            Total = 1,
            Items = [UserDtoBuilder.Valid().Build()],
        };
        var cancellationToken = TestContext.Current.CancellationToken;

        _userRepository.GetByPageAsync<UserDto>(Arg.Any<UserPaginatedSpec>(), cancellationToken)
            .Returns(expected);

        // Act
        var result = await _userService.GetUsersByPageAsync(options, cancellationToken);

        // Assert
        Assert.Same(expected, result);
        await _userRepository.Received(1)
            .GetByPageAsync<UserDto>(Arg.Any<UserPaginatedSpec>(), cancellationToken);
    }

    [Fact]
    public async Task GivenPagingOptions_WhenKeywordSupplied_ThenPassesSpecificationBuiltFromOptions()
    {
        // Arrange
        var options = new UserPaginatedOptions(page: 2, size: 5) { Keyword = "ruong" };
        var cancellationToken = TestContext.Current.CancellationToken;

        _userRepository.GetByPageAsync<UserDto>(Arg.Any<UserPaginatedSpec>(), cancellationToken)
            .Returns(new PaginatedModel<UserDto> { Page = 2, Count = 0, Total = 0, Items = [] });

        // Act
        await _userService.GetUsersByPageAsync(options, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByPageAsync<UserDto>(
            Arg.Is<UserPaginatedSpec>(spec =>
                spec.PaginatedOptions.Page == 2 && spec.PaginatedOptions.Size == 5),
            cancellationToken);
    }
}
