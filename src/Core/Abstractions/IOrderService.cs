using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Core.Abstractions;

public interface IOrderService
{
    Task<OrderDto?> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <exception cref="InvalidOperationException"></exception>"
    Task CreateOrderAsync(OrderDto model, CancellationToken cancellationToken = default);

    /// <exception cref="InvalidOperationException"></exception>"
    Task RemoveOrderAsync(int id, CancellationToken cancellationToken = default);

    /// <exception cref="InvalidOperationException"></exception>"
    Task UpdateOrderAsync(OrderDto model, CancellationToken cancellationToken = default);
}