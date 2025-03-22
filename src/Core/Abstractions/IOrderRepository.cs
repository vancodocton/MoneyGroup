using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Core.Abstractions;
public interface IOrderRepository
    : IRepository<Order>
{
    Task<OrderDto> UpdateAsync(OrderDto dto, CancellationToken cancellationToken = default);
}