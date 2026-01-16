using Ardalis.Specification;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Exceptions;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Specifications;

namespace MoneyGroup.Core.Services;

public class OrderService
    : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
    }

    public async Task<OrderDetailedDto?> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _orderRepository.FirstOrDefaultAsync<OrderDetailedDto>(new EntityByIdSpec<Order>(id), cancellationToken);
    }

    /// <inheritdoc />
    public async Task CreateOrderAsync(OrderDto model, CancellationToken cancellationToken = default)
    {
        if (!await _userRepository.AnyAsync(new EntityByIdSpec<User>(model.BuyerId), cancellationToken))
        {
            throw new BuyerNotFoundException();
        }

        var participantIds = model.Participants.Select(c => c.ParticipantId).ToList();
        if (participantIds.Count > 0)
        {
            var existingParticipantsCount = await _userRepository.CountAsync(
                new EntityByIdsSpec<User>(participantIds),
                cancellationToken);

            if (existingParticipantsCount != participantIds.Count)
            {
                throw new ParticipantNotFoundException();
            }
        }

        await _orderRepository.AddAsync(model, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<bool> RemoveOrderAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.FirstOrDefaultAsync(new EntityByIdSpec<Order>(id), cancellationToken);
        if (order == null)
        {
            return false;
        }

        await _orderRepository.RemoveAsync(order, cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public Task<PaginatedModel<OrderDetailedDto>> GetOrdersByPageAsync(OrderPaginatedOptions options)
    {
        return _orderRepository.GetByPageAsync<OrderDetailedDto>(new OrderPaginatedSpec(options));
    }
}
