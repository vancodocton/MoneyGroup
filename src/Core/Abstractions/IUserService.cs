using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Models.Users;

namespace MoneyGroup.Core.Abstractions;

/// <summary>
/// Provides user-related operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves a paginated list of users based on the specified options.
    /// </summary>
    /// <param name="options">The pagination options for retrieving users.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paginated model of user data transfer objects.</returns>
    Task<PaginatedModel<UserDto>> GetUsersByPageAsync(UserPaginatedOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user data transfer object if found; otherwise, null.</returns>
    Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user data transfer object if found; otherwise, null.</returns>
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
}
