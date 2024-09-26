using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Core.Abstractions;
public interface IOrderRepository
    : IRepository<Order>
{
    public Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default);

    public Task<Order?> FirstOrDefaultAsync(int id, CancellationToken cancellationToken = default);

    public Task<TResult?> FirstOrDefaultAsync<TResult>(int id, CancellationToken cancellationToken = default);
    
    Task<OrderDto> UpdateAsync(OrderDto dto, CancellationToken cancellationToken = default);
}