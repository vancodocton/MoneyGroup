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

        var idsHashSet = new HashSet<int>();

        foreach (var participantId in model.Participants.Select(c => c.Id))
        {
            if (!await _userRepository.AnyAsync(new EntityByIdSpec<User>(participantId), cancellationToken))
            {
                throw new ParticipantNotFoundException();
            }

            if (!idsHashSet.Add(participantId))
            {
                throw new ParticipantDuplicatedException();
            }
        }

        await _orderRepository.AddAsync(model, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateOrderAsync(OrderDto model, CancellationToken cancellationToken = default)
    {
        if (!await _orderRepository.AnyAsync(new EntityByIdSpec<Order>(model.Id), cancellationToken))
        {
            throw new OrderNotFoundException();
        }

        if (!await _userRepository.AnyAsync(new EntityByIdSpec<User>(model.BuyerId), cancellationToken))
        {
            throw new BuyerNotFoundException();
        }

        var idsHashSet = new HashSet<int>();

        foreach (var participantId in model.Participants.Select(c => c.Id))
        {
            if (!await _userRepository.AnyAsync(new EntityByIdSpec<User>(participantId), cancellationToken))
            {
                throw new ParticipantNotFoundException();
            }

            if (!idsHashSet.Add(participantId))
            {
                throw new ParticipantDuplicatedException();
            }
        }

        await _orderRepository.UpdateAsync(model, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveOrderAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.FirstOrDefaultAsync(new EntityByIdSpec<Order>(id), cancellationToken);
        if (order == null)
        {
            throw new OrderNotFoundException();
        }

        await _orderRepository.RemoveAsync(order, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PaginatedModel<OrderDetailedDto>> GetOrdersByPageAsync(OrderPaginatedOptions options)
    {
        return _orderRepository.GetByPageAsync<OrderDetailedDto>(new OrderPaginatedSpec(options));
    }
}