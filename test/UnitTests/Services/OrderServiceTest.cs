using System.Linq.Expressions;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Exceptions;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Services;
using MoneyGroup.Core.Specifications;

using NSubstitute;

namespace MoneyGroup.UnitTests.Services;

public class OrderServiceTest
{
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private readonly IOrderService _orderService;
#pragma warning restore CA1859 // Use concrete types when possible for improved performance

    public OrderServiceTest()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _orderRepository = Substitute.For<IOrderRepository>();
        _orderService = new OrderService(_orderRepository, _userRepository);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ValidId_ShouldReturnOrder()
    {
        // Arrange
        int id = 1;
        var orderDto = new OrderDetailedDto
        {
            Id = id,
        };
        CancellationToken cancellationToken = default;

        _orderRepository.FirstOrDefaultAsync<OrderDetailedDto>(Arg.Any<EntityByIdSpec<Order>>(), cancellationToken)
            .Returns(orderDto);

        // Act
        var result = await _orderService.GetOrderByIdAsync(id, cancellationToken);

        // Assert
        await _orderRepository.Received().FirstOrDefaultAsync<OrderDetailedDto>(Arg.Any<EntityByIdSpec<Order>>(), cancellationToken);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetOrderByIdAsync_InvalidId_ShouldReturnNull()
    {
        // Arrange
        int invalidId = -1;
        CancellationToken cancellationToken = default;

        _orderRepository.FirstOrDefaultAsync<OrderDetailedDto>(Arg.Any<Expression<Func<Order, bool>>>(), cancellationToken)
            .Returns((OrderDetailedDto?)null);

        // Act
        var result = await _orderService.GetOrderByIdAsync(invalidId, cancellationToken);

        // Assert
        await _orderRepository.Received().FirstOrDefaultAsync<OrderDetailedDto>(Arg.Any<EntityByIdSpec<Order>>(), cancellationToken);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateOrderAsync_ValidDto_ShouldAddOrder()
    {
        // Arrange
        var newOrderId = 1;
        var model = new OrderDto
        {
            BuyerId = 1,
            Participants =
            [
                new() { ParticipantId = 2 },
                new() { ParticipantId = 3 },
            ],
        };

        _userRepository.AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        _userRepository.CountAsync(Arg.Any<EntityByIdsSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(2);

        _orderRepository.AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                model.Id = newOrderId;
                return model;
            });

        // Act
        await _orderService.CreateOrderAsync(model, TestContext.Current.CancellationToken);

        // Assert
        await _userRepository.Received(1).AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>());
        await _userRepository.Received(1).CountAsync(Arg.Any<EntityByIdsSpec<User>>(), Arg.Any<CancellationToken>());
        await _orderRepository.Received().AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>());
        Assert.Equal(newOrderId, model.Id);
    }

    [Fact]
    public async Task CreateOrderAsync_NoParticipants_ShouldAddOrder()
    {
        // Arrange
        var newOrderId = 1;
        var model = new OrderDto
        {
            BuyerId = 1,
            Participants = []
        };

        _userRepository.AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        _orderRepository.AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                model.Id = newOrderId;
                return model;
            });

        // Act
        await _orderService.CreateOrderAsync(model, TestContext.Current.CancellationToken);

        // Assert
        await _userRepository.Received(1).AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceive().CountAsync(Arg.Any<EntityByIdsSpec<User>>(), Arg.Any<CancellationToken>());
        await _orderRepository.Received().AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>());
        Assert.Equal(newOrderId, model.Id);
    }

    [Fact]
    public async Task CreateOrderAsync_InvalidBuyer_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            BuyerId = -1,
            Participants =
            [
                new() { ParticipantId = 2 },
                new() { ParticipantId = 3 },
            ]
        };

        _userRepository.AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var ex = await Assert.ThrowsAsync<BuyerNotFoundException>(() => _orderService.CreateOrderAsync(model, TestContext.Current.CancellationToken));

        // Assert
        await _userRepository.Received().AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>());
        await _orderRepository.DidNotReceive().AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>());
        Assert.Equal("Buyer not found", ex.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_InvalidParticipants_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            BuyerId = 1,
            Participants =
            [
                new() { ParticipantId = 2 },
                new() { ParticipantId = -1 },
            ]
        };

        _userRepository.AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        _userRepository.CountAsync(Arg.Any<EntityByIdsSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(1); // Only 1 participant exists, but 2 were requested

        // Act
        var ex = await Assert.ThrowsAsync<ParticipantNotFoundException>(() => _orderService.CreateOrderAsync(model, TestContext.Current.CancellationToken));

        // Assert
        await _userRepository.Received(1).AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>());
        await _userRepository.Received(1).CountAsync(Arg.Any<EntityByIdsSpec<User>>(), Arg.Any<CancellationToken>());
        await _orderRepository.DidNotReceive().AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>());
        Assert.Equal("Participant not found", ex.Message);
    }

    [Fact]
    public async Task RemoveOrderAsync_OrderExists_ShouldRemoveOrder()
    {
        // Arrange
        var orderId = 1;
        var order = new Order { Id = orderId };

        _orderRepository.FirstOrDefaultAsync(Arg.Any<EntityByIdSpec<Order>>(), Arg.Any<CancellationToken>())
            .Returns(order);

        _orderRepository.RemoveAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _orderService.RemoveOrderAsync(orderId, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        await _orderRepository.Received().FirstOrDefaultAsync(Arg.Any<EntityByIdSpec<Order>>(), Arg.Any<CancellationToken>());
        await _orderRepository.Received().RemoveAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveOrderAsync_OrderNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var orderId = 1;

        _orderRepository.FirstOrDefaultAsync(Arg.Any<EntityByIdSpec<Order>>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        // Act
        var result = await _orderService.RemoveOrderAsync(orderId, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
        await _orderRepository.Received().FirstOrDefaultAsync(Arg.Any<EntityByIdSpec<Order>>(), Arg.Any<CancellationToken>());
    }
}
