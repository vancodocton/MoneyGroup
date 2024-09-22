using MoneyGroup.Core.Entities;

namespace MoneyGroup.Core.Abstractions;
public interface IOrderRepository
    : IRepository<Order>
{
    public Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default);

    public Task<Order?> FirstOrDefaultAsync(int id, CancellationToken cancellationToken = default);

    public Task<TResult?> FirstOrDefaultAsync<TResult>(int id, CancellationToken cancellationToken = default);
}