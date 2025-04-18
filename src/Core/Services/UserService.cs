using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.Core.Specifications;

namespace MoneyGroup.Core.Services;

public class UserService(IUserRepository userRepository)
    : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;


    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _userRepository.FirstOrDefaultAsync<UserDto>(new UserByEmailSpec(email), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _userRepository.FirstOrDefaultAsync<UserDto>(new EntityByIdSpec<User>(id), cancellationToken);
    }

    /// <inheritdoc />
    public Task<PaginatedModel<UserDto>> GetUsersByPageAsync(UserPaginatedOptions options, CancellationToken cancellationToken = default)
    {
        return _userRepository.GetByPageAsync<UserDto>(new UserPaginatedSpec(options), cancellationToken);
    }
}
