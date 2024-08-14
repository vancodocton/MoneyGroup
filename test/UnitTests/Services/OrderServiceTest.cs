using System.Linq.Expressions;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Services;

using Moq;

namespace MoneyGroup.UnitTests.Services;
public class OrderServiceTest
{
    private readonly Mock<IRepository<User>> _userRepository;
    private readonly Mock<IRepository<Order>> _orderRepository;
    private readonly IOrderService _orderService;

    public OrderServiceTest()
    {
        _userRepository = new Mock<IRepository<User>>();
        _orderRepository = new Mock<IRepository<Order>>();
        _orderService = new OrderService(_orderRepository.Object, _userRepository.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_ValidDto_ShouldAddOrder()
    {
        // Arrange
        var newOrderId = 1;
        var model = new OrderDto
        {
            IssuerId = 1,
            Consumers =
            [
                new() { Id = 2 },
                new() { Id = 3 },
            ],
        };

        _userRepository.Setup(u => u.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _orderRepository.Setup(o => o.AddAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                model.Id = newOrderId;
                return model;
            });

        // Act
        await _orderService.CreateOrderAsync(model);

        // Assert
        _userRepository.Verify(u => u.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _orderRepository.Verify(o => o.AddAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(newOrderId, model.Id);
    }

    [Fact]
    public async Task CreateOrderAsync_InvalidIssuer_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            IssuerId = -1,
            Consumers =
            [
                new() { Id = 2 },
                new() { Id = 3 },
            ]
        };

        _userRepository.Setup(u => u.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.CreateOrderAsync(model));

        // Assert
        Assert.Equal("Issuer not found", ex.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_InvalidConsumers_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            IssuerId = 1,
            Consumers =
            [
                new() { Id = 2 },
                new() { Id = -1 },
            ]
        };

        _userRepository.SetupSequence(u => u.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.CreateOrderAsync(model));

        // Assert
        Assert.Equal("Consumer not found", ex.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_DuplicateConsumers_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            IssuerId = 1,
            Consumers =
            [
                new() { Id = 2 },
                new() { Id = 2 },
            ]
        };

        _userRepository.Setup(u => u.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.CreateOrderAsync(model));

        // Assert
        Assert.Equal("Duplicated consumer", ex.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_EmptyConsumers_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            IssuerId = 1,
            Consumers = [],
        };
        _userRepository.Setup(u => u.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.CreateOrderAsync(model));

        // Assert
        Assert.Equal("Consumers is empty", ex.Message);
    }

    [Fact]
    public async Task UpdateOrderAsync_ValidDto_ShouldUpdateOrder()
    {
        // Arrange
        var model = new OrderDto
        {
            Id = 1,
            IssuerId = 1,
            Consumers =
                [
                    new() { Id = 2 },
                    new() { Id = 3 },
                ]
        };

        _orderRepository.Setup(o => o.AnyAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepository.Setup(u => u.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _orderRepository.Setup(o => o.UpdateAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<OrderDto>(null!));

        // Act
        await _orderService.UpdateOrderAsync(model);

        // Assert
        _orderRepository.Verify(o => o.AnyAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _userRepository.Verify(u => u.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _orderRepository.Verify(o => o.UpdateAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderAsync_InvalidIssuer_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            Id = 1,
            IssuerId = -1,
            Consumers =
            [
                new() { Id = 2 },
                new() { Id = 3 },
            ]
        };

        _orderRepository.Setup(o => o.AnyAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepository.Setup(u => u.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.UpdateOrderAsync(model));

        // Assert
        Assert.Equal("Issuer not found", ex.Message);
    }

    [Fact]
    public async Task UpdateOrderAsync_OrderNotExisted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            Id = -1,
            IssuerId = 1,
            Consumers = new List<ConsumerDto>
            {
                new() { Id = 2 },
                new() { Id = 3 }
            }
        };

        _orderRepository.Setup(o => o.AnyAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.UpdateOrderAsync(model));

        //  Assert
        Assert.Equal("Order not found", ex.Message);
    }

    [Fact]
    public async Task UpdateOrderAsync_InvalidConsumers_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            Id = 1,
            IssuerId = 1,
            Consumers =
            [
                new() { Id = 2 },
                new() { Id = -1 },
            ]
        };

        _orderRepository.Setup(o => o.AnyAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepository.SetupSequence(u => u.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.UpdateOrderAsync(model));

        // Assert
        Assert.Equal("Consumer not found", ex.Message);
    }

    [Fact]
    public async Task UpdateOrderAsync_EmptyConsumers_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            Id = 1,
            IssuerId = 1,
            Consumers = [],
        };

        _orderRepository.Setup(o => o.AnyAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepository.Setup(u => u.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.UpdateOrderAsync(model));

        // Assert
        Assert.Equal("Consumers is empty", ex.Message);
    }

    [Fact]
    public async Task UpdateOrderAsync_DuplicateConsumers_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            Id = 1,
            IssuerId = 1,
            Consumers =
            [
                new() { Id = 2 },
                new() { Id = 2 },
            ]
        };

        _orderRepository.Setup(o => o.AnyAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepository.Setup(u => u.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.UpdateOrderAsync(model));

        // Assert
        Assert.Equal("Duplicated consumer", ex.Message);
    }

    [Fact]
    public async Task RemoveOrderAsync_OrderExists_ShouldRemoveOrder()
    {
        // Arrange
        var orderId = 1;
        var order = new Order { Id = orderId };

        _orderRepository.Setup(o => o.FirstOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _orderRepository.Setup(o => o.RemoveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _orderService.RemoveOrderAsync(orderId);

        // Assert
        _orderRepository.Verify(o => o.FirstOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _orderRepository.Verify(o => o.RemoveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveOrderAsync_OrderNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var orderId = 1;

        _orderRepository.Setup(o => o.FirstOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.RemoveOrderAsync(orderId));
        Assert.Equal("Order not found", ex.Message);
    }
}