using MoneyGroup.Core.Exceptions;
using MoneyGroup.Core.Models;
using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Core.Abstractions;

public interface IOrderService
{
    Task<OrderDetailedDto?> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new order.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="BuyerNotFoundException"></exception>
    /// <exception cref="ParticipantNotFoundException"></exception>
    /// <exception cref="ParticipantDuplicatedException"></exception>
    Task CreateOrderAsync(OrderDto model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update order.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="OrderNotFoundException"></exception>
    /// <exception cref="BuyerNotFoundException"></exception>
    /// <exception cref="ParticipantNotFoundException"></exception>
    /// <exception cref="ParticipantDuplicatedException"></exception>
    Task UpdateOrderAsync(OrderDto model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove order by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="OrderNotFoundException"></exception>
    Task RemoveOrderAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get orders by page.
    /// </summary>
    /// <param name="options">Pagination options for retrieving orders by page.</param>
    /// <returns></returns>
    Task<PaginatedModel<OrderDetailedDto>> GetOrdersByPageAsync(IPaginatedOptions options);
}