using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Core.Services;

public class OrderService
    : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<User> _userRepository;


    public OrderService(IRepository<Order> orderRepository, IRepository<User> userRepository)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
    }

    public async Task CreateOrderAsync(OrderDto model, CancellationToken cancellationToken = default)
    {
        if (!await _userRepository.AnyAsync(u => u.Id == model.IssuerId, cancellationToken))
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
            if (!await _userRepository.AnyAsync(u => u.Id == consumerId, cancellationToken))
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
        if (!await _orderRepository.AnyAsync(o => o.Id == model.Id, cancellationToken))
        {
            throw new InvalidOperationException("Order not found");
        }

        if (!await _userRepository.AnyAsync(u => u.Id == model.IssuerId, cancellationToken))
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
            if (!await _userRepository.AnyAsync(u => u.Id == consumerId, cancellationToken))
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
        var order = await _orderRepository.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        if (order == null)
        {
            throw new InvalidOperationException("Order not found");
        }

        await _orderRepository.RemoveAsync(order, cancellationToken);
    }
}