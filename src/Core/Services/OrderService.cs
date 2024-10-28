using FluentValidation;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Core.Services;

public class OrderService
    : IOrderService
{
    private readonly IValidator<OrderDto> _orderValidator;
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;

    public OrderService(
        IValidator<OrderDto> orderValidator,
        IOrderRepository orderRepository,
        IUserRepository userRepository)
    {
        _orderValidator = orderValidator;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _orderRepository.FirstOrDefaultAsync<OrderDto>(id, cancellationToken);
    }

    public async Task CreateOrderAsync(OrderDto model, CancellationToken cancellationToken = default)
    {
        await _orderValidator.ValidateAndThrowAsync(model, cancellationToken: cancellationToken);

        if (!await _userRepository.AnyAsync(model.IssuerId, cancellationToken))
        {
            throw new InvalidOperationException("Issuer not found");
        }

        if (!model.Consumers.Any())
        {
            throw new InvalidOperationException("Consumers is empty");
        }

        var idsHashSet = new HashSet<int>();

        foreach (var consumerId in model.Consumers.Select(c => c.Id))
        {
            if (!await _userRepository.AnyAsync(consumerId, cancellationToken))
            {
                throw new InvalidOperationException("Consumer not found");
            }

            if (!idsHashSet.Add(consumerId))
            {
                throw new InvalidOperationException($"Duplicated consumer");
            }
        }

        await _orderRepository.AddAsync(model, cancellationToken);
    }

    public async Task UpdateOrderAsync(OrderDto model, CancellationToken cancellationToken = default)
    {
        await _orderValidator.ValidateAndThrowAsync(model, cancellationToken: cancellationToken);

        if (!await _orderRepository.AnyAsync(model.Id, cancellationToken))
        {
            throw new InvalidOperationException("Order not found");
        }

        if (!await _userRepository.AnyAsync(model.IssuerId, cancellationToken))
        {
            throw new InvalidOperationException("Issuer not found");
        }

        if (!model.Consumers.Any())
        {
            throw new InvalidOperationException("Consumers is empty");
        }

        var idsHashSet = new HashSet<int>();

        foreach (var consumerId in model.Consumers.Select(c => c.Id))
        {
            if (!await _userRepository.AnyAsync(consumerId, cancellationToken))
            {
                throw new InvalidOperationException("Consumer not found");
            }

            if (!idsHashSet.Add(consumerId))
            {
                throw new InvalidOperationException($"Duplicated consumer");
            }
        }

        await _orderRepository.UpdateAsync(model, cancellationToken);
    }

    public async Task RemoveOrderAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.FirstOrDefaultAsync(id, cancellationToken);
        if (order == null)
        {
            throw new InvalidOperationException("Order not found");
        }

        await _orderRepository.RemoveAsync(order, cancellationToken);
    }
}