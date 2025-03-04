using Ardalis.Specification;

using FluentValidation;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Exceptions;
using MoneyGroup.Core.Models;
using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Core.Services;

public class OrderService
    : IOrderService
{
    private readonly IValidator<OrderDto> _orderValidator;
    private readonly IValidator<IPaginationOptions> _paginatedOptionsValidator;
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;

    public OrderService(
        IValidator<OrderDto> orderValidator,
        IValidator<IPaginationOptions> paginatedOptionsValidator,
        IOrderRepository orderRepository,
        IUserRepository userRepository)
    {
        _orderValidator = orderValidator;
        _paginatedOptionsValidator = paginatedOptionsValidator;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _orderRepository.FirstOrDefaultAsync<OrderDto>(id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task CreateOrderAsync(OrderDto model, CancellationToken cancellationToken = default)
    {
        await _orderValidator.ValidateAndThrowAsync(model, cancellationToken: cancellationToken);

        if (!await _userRepository.AnyAsync(model.BuyerId, cancellationToken))
        {
            throw new BuyerNotFoundException();
        }

        var idsHashSet = new HashSet<int>();

        foreach (var participantId in model.Participants.Select(c => c.Id))
        {
            if (!await _userRepository.AnyAsync(participantId, cancellationToken))
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
        await _orderValidator.ValidateAndThrowAsync(model, cancellationToken: cancellationToken);

        if (!await _orderRepository.AnyAsync(model.Id, cancellationToken))
        {
            throw new OrderNotFoundException();
        }

        if (!await _userRepository.AnyAsync(model.BuyerId, cancellationToken))
        {
            throw new BuyerNotFoundException();
        }

        var idsHashSet = new HashSet<int>();

        foreach (var participantId in model.Participants.Select(c => c.Id))
        {
            if (!await _userRepository.AnyAsync(participantId, cancellationToken))
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
        var order = await _orderRepository.FirstOrDefaultAsync(id, cancellationToken);
        if (order == null)
        {
            throw new OrderNotFoundException();
        }

        await _orderRepository.RemoveAsync(order, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PaginationModel<OrderDto>> GetOrdersByPageAsync(IPaginationOptions options)
    {
        _paginatedOptionsValidator.ValidateAndThrow(options);
        return _orderRepository.GetByPageAsync<OrderDto>(new OrderPaginatedSpec(options));
    }
}