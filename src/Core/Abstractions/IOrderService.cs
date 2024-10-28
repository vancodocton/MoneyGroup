using MoneyGroup.Core.Exceptions;
using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Core.Abstractions;

public interface IOrderService
{
    Task<OrderDto?> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new order.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="IssuerNotFoundException"></exception>
    /// <exception cref="ConsumerNotFoundException"></exception>
    /// <exception cref="ConsumerDuplicatedException"></exception>
    Task CreateOrderAsync(OrderDto model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update order.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="OrderNotFoundException"></exception>
    /// <exception cref="IssuerNotFoundException"></exception>
    /// <exception cref="ConsumerNotFoundException"></exception>
    /// <exception cref="ConsumerDuplicatedException"></exception>
    Task UpdateOrderAsync(OrderDto model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove order by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="OrderNotFoundException"></exception>
    Task RemoveOrderAsync(int id, CancellationToken cancellationToken = default);
}