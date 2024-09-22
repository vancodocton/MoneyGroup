using MoneyGroup.Core.Entities;

namespace MoneyGroup.Core.Abstractions;
public interface IUserRepository
    : IRepository<User>
{
    public Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default);
}