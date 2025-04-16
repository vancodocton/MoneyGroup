using MoneyGroup.Core.Exceptions;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Models.Paginations;

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
    Task CreateOrderAsync(OrderDto model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove order by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<bool> RemoveOrderAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get orders by page.
    /// </summary>
    /// <param name="options">Pagination options for retrieving orders by page.</param>
    /// <returns></returns>
    Task<PaginatedModel<OrderDetailedDto>> GetOrdersByPageAsync(OrderPaginatedOptions options);
}