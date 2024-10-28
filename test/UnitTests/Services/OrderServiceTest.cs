using System.Linq.Expressions;

using FluentValidation;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Exceptions;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Services;

using Moq;

namespace MoneyGroup.UnitTests.Services;
public class OrderServiceTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IValidator<OrderDto>> _orderValidatorMock;
#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private readonly IOrderService _orderService;
#pragma warning restore CA1859 // Use concrete types when possible for improved performance

    public OrderServiceTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderValidatorMock = new Mock<IValidator<OrderDto>>();
        _orderService = new OrderService(_orderValidatorMock.Object, _orderRepositoryMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ValidId_ShouldReturnOrder()
    {
        // Arrange
        int id = 1;
        var orderDto = new OrderDto
        {
            Id = id,
        };
        CancellationToken cancellationToken = default;

        _orderRepositoryMock.Setup(o => o.FirstOrDefaultAsync<OrderDto>(It.IsAny<int>(), cancellationToken))
            .ReturnsAsync(orderDto);

        // Act
        var result = await _orderService.GetOrderByIdAsync(id, cancellationToken);

        // Assert
        _orderRepositoryMock.Verify(o => o.FirstOrDefaultAsync<OrderDto>(It.IsAny<int>(), cancellationToken));
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetOrderByIdAsync_InvalidId_ShouldReturnNull()
    {
        // Arrange
        int invalidId = -1;
        CancellationToken cancellationToken = default;

        _orderRepositoryMock.Setup(o => o.FirstOrDefaultAsync<OrderDto>(It.IsAny<Expression<Func<Order, bool>>>(), cancellationToken))
            .ReturnsAsync((OrderDto?)null);

        // Act
        var result = await _orderService.GetOrderByIdAsync(invalidId, cancellationToken);

        // Assert
        _orderRepositoryMock.Verify(o => o.FirstOrDefaultAsync<OrderDto>(It.IsAny<int>(), cancellationToken));
        Assert.Null(result);
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

        _userRepositoryMock.Setup(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _orderRepositoryMock.Setup(o => o.AddAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                model.Id = newOrderId;
                return model;
            });

        // Act
        await _orderService.CreateOrderAsync(model);

        // Assert
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _orderRepositoryMock.Verify(o => o.AddAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()));
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

        _userRepositoryMock.Setup(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<IssuerNotFoundException>(() => _orderService.CreateOrderAsync(model));

        // Assert
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
        _orderRepositoryMock.Verify(o => o.AddAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()), Times.Never);
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

        _userRepositoryMock.SetupSequence(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<ConsumerNotFoundException>(() => _orderService.CreateOrderAsync(model));

        // Assert
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _orderRepositoryMock.Verify(o => o.AddAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()), Times.Never);
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

        _userRepositoryMock.Setup(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var ex = await Assert.ThrowsAsync<ConsumerDuplicatedException>(() => _orderService.CreateOrderAsync(model));

        // Assert
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _orderRepositoryMock.Verify(o => o.AddAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal("Duplicated consumer", ex.Message);
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

        _orderRepositoryMock.Setup(o => o.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock.Setup(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _orderRepositoryMock.Setup(o => o.UpdateAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<OrderDto>(null!));

        // Act
        await _orderService.UpdateOrderAsync(model);

        // Assert
        _orderRepositoryMock.Verify(o => o.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _orderRepositoryMock.Verify(o => o.UpdateAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()));
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

        _orderRepositoryMock.Setup(o => o.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock.Setup(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<IssuerNotFoundException>(() => _orderService.UpdateOrderAsync(model));

        // Assert
        _orderRepositoryMock.Verify(o => o.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
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
            Consumers =
            [
                new() { Id = 2 },
                new() { Id = 3 }
            ]
        };

        _orderRepositoryMock.Setup(o => o.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<OrderNotFoundException>(() => _orderService.UpdateOrderAsync(model));

        //  Assert
        _orderRepositoryMock.Verify(o => o.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
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

        _orderRepositoryMock.Setup(o => o.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock.SetupSequence(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<ConsumerNotFoundException>(() => _orderService.UpdateOrderAsync(model));

        // Assert
        _orderRepositoryMock.Verify(o => o.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        Assert.Equal("Consumer not found", ex.Message);
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

        _orderRepositoryMock.Setup(o => o.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock.Setup(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var ex = await Assert.ThrowsAsync<ConsumerDuplicatedException>(() => _orderService.UpdateOrderAsync(model));

        // Assert
        _orderRepositoryMock.Verify(o => o.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        Assert.Equal("Duplicated consumer", ex.Message);
    }

    [Fact]
    public async Task RemoveOrderAsync_OrderExists_ShouldRemoveOrder()
    {
        // Arrange
        var orderId = 1;
        var order = new Order { Id = orderId };

        _orderRepositoryMock.Setup(o => o.FirstOrDefaultAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _orderRepositoryMock.Setup(o => o.RemoveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _orderService.RemoveOrderAsync(orderId);

        // Assert
        _orderRepositoryMock.Verify(o => o.FirstOrDefaultAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
        _orderRepositoryMock.Verify(o => o.RemoveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task RemoveOrderAsync_OrderNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var orderId = 1;

        _orderRepositoryMock.Setup(o => o.FirstOrDefaultAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var ex = await Assert.ThrowsAsync<OrderNotFoundException>(() => _orderService.RemoveOrderAsync(orderId));

        // Assert
        _orderRepositoryMock.Verify(o => o.FirstOrDefaultAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
        Assert.Equal("Order not found", ex.Message);
    }
}