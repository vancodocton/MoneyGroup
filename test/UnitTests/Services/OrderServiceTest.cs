using System.Linq.Expressions;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Exceptions;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Services;
using MoneyGroup.Core.Specifications;

using Moq;

namespace MoneyGroup.UnitTests.Services;
public class OrderServiceTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private readonly IOrderService _orderService;
#pragma warning restore CA1859 // Use concrete types when possible for improved performance

    public OrderServiceTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderService = new OrderService(_orderRepositoryMock.Object, _userRepositoryMock.Object);
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

        _orderRepositoryMock.Setup(o => o.FirstOrDefaultAsync<OrderDetailedDto>(It.IsAny<EntityByIdSpec<Order>>(), cancellationToken))
            .ReturnsAsync(orderDto);

        // Act
        var result = await _orderService.GetOrderByIdAsync(id, cancellationToken);

        // Assert
        _orderRepositoryMock.Verify(o => o.FirstOrDefaultAsync<OrderDetailedDto>(It.IsAny<EntityByIdSpec<Order>>(), cancellationToken));
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetOrderByIdAsync_InvalidId_ShouldReturnNull()
    {
        // Arrange
        int invalidId = -1;
        CancellationToken cancellationToken = default;

        _orderRepositoryMock.Setup(o => o.FirstOrDefaultAsync<OrderDetailedDto>(It.IsAny<Expression<Func<Order, bool>>>(), cancellationToken))
            .ReturnsAsync((OrderDetailedDto?)null);

        // Act
        var result = await _orderService.GetOrderByIdAsync(invalidId, cancellationToken);

        // Assert
        _orderRepositoryMock.Verify(o => o.FirstOrDefaultAsync<OrderDetailedDto>(It.IsAny<EntityByIdSpec<Order>>(), cancellationToken));
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

        _userRepositoryMock.Setup(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _orderRepositoryMock.Setup(o => o.AddAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                model.Id = newOrderId;
                return model;
            });

        // Act
        await _orderService.CreateOrderAsync(model, TestContext.Current.CancellationToken);

        // Assert
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _orderRepositoryMock.Verify(o => o.AddAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()));
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

        _userRepositoryMock.Setup(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<BuyerNotFoundException>(() => _orderService.CreateOrderAsync(model, TestContext.Current.CancellationToken));

        // Assert
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()));
        _orderRepositoryMock.Verify(o => o.AddAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()), Times.Never);
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

        _userRepositoryMock.SetupSequence(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<ParticipantNotFoundException>(() => _orderService.CreateOrderAsync(model, TestContext.Current.CancellationToken));

        // Assert
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _orderRepositoryMock.Verify(o => o.AddAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal("Participant not found", ex.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_DuplicateParticipants_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            BuyerId = 1,
            Participants =
            [
                new() { ParticipantId = 2 },
                new() { ParticipantId = 2 },
            ]
        };

        _userRepositoryMock.Setup(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var ex = await Assert.ThrowsAsync<ParticipantDuplicatedException>(() => _orderService.CreateOrderAsync(model, TestContext.Current.CancellationToken));

        // Assert
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _orderRepositoryMock.Verify(o => o.AddAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal("Duplicated participant", ex.Message);
    }

    [Fact]
    public async Task UpdateOrderAsync_ValidDto_ShouldUpdateOrder()
    {
        // Arrange
        var model = new OrderDto
        {
            Id = 1,
            BuyerId = 1,
            Participants =
                [
                    new() { ParticipantId = 2 },
                    new() { ParticipantId = 3 },
                ]
        };

        _orderRepositoryMock.Setup(o => o.AnyAsync(It.IsAny<EntityByIdSpec<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock.Setup(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _orderRepositoryMock.Setup(o => o.UpdateAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<OrderDto>(null!));

        // Act
        await _orderService.UpdateOrderAsync(model, TestContext.Current.CancellationToken);

        // Assert
        _orderRepositoryMock.Verify(o => o.AnyAsync(It.IsAny<EntityByIdSpec<Order>>(), It.IsAny<CancellationToken>()));
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _orderRepositoryMock.Verify(o => o.UpdateAsync(It.IsAny<OrderDto>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task UpdateOrderAsync_InvalidBuyer_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            Id = 1,
            BuyerId = -1,
            Participants =
            [
                new() { ParticipantId = 2 },
                new() { ParticipantId = 3 },
            ]
        };

        _orderRepositoryMock.Setup(o => o.AnyAsync(It.IsAny<EntityByIdSpec<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock.Setup(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<BuyerNotFoundException>(() => _orderService.UpdateOrderAsync(model, TestContext.Current.CancellationToken));

        // Assert
        _orderRepositoryMock.Verify(o => o.AnyAsync(It.IsAny<EntityByIdSpec<Order>>(), It.IsAny<CancellationToken>()));
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()));
        Assert.Equal("Buyer not found", ex.Message);
    }

    [Fact]
    public async Task UpdateOrderAsync_OrderNotExisted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            Id = -1,
            BuyerId = 1,
            Participants =
            [
                new() { ParticipantId = 2 },
                new() { ParticipantId = 3 }
            ]
        };

        _orderRepositoryMock.Setup(o => o.AnyAsync(It.IsAny<EntityByIdSpec<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<OrderNotFoundException>(() => _orderService.UpdateOrderAsync(model, TestContext.Current.CancellationToken));

        //  Assert
        _orderRepositoryMock.Verify(o => o.AnyAsync(It.IsAny<EntityByIdSpec<Order>>(), It.IsAny<CancellationToken>()));
        Assert.Equal("Order not found", ex.Message);
    }

    [Fact]
    public async Task UpdateOrderAsync_InvalidParticipants_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            Id = 1,
            BuyerId = 1,
            Participants =
            [
                new() { ParticipantId = 2 },
                new() { ParticipantId = -1 },
            ]
        };

        _orderRepositoryMock.Setup(o => o.AnyAsync(It.IsAny<EntityByIdSpec<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock.SetupSequence(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // Act
        var ex = await Assert.ThrowsAsync<ParticipantNotFoundException>(() => _orderService.UpdateOrderAsync(model, TestContext.Current.CancellationToken));

        // Assert
        _orderRepositoryMock.Verify(o => o.AnyAsync(It.IsAny<EntityByIdSpec<Order>>(), It.IsAny<CancellationToken>()));
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        Assert.Equal("Participant not found", ex.Message);
    }

    [Fact]
    public async Task UpdateOrderAsync_DuplicateParticipants_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var model = new OrderDto
        {
            Id = 1,
            BuyerId = 1,
            Participants =
            [
                new() { ParticipantId = 2 },
                new() { ParticipantId = 2 },
            ]
        };

        _orderRepositoryMock.Setup(o => o.AnyAsync(It.IsAny<EntityByIdSpec<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock.Setup(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var ex = await Assert.ThrowsAsync<ParticipantDuplicatedException>(() => _orderService.UpdateOrderAsync(model, TestContext.Current.CancellationToken));

        // Assert
        _orderRepositoryMock.Verify(o => o.AnyAsync(It.IsAny<EntityByIdSpec<Order>>(), It.IsAny<CancellationToken>()));
        _userRepositoryMock.Verify(u => u.AnyAsync(It.IsAny<EntityByIdSpec<User>>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        Assert.Equal("Duplicated participant", ex.Message);
    }

    [Fact]
    public async Task RemoveOrderAsync_OrderExists_ShouldRemoveOrder()
    {
        // Arrange
        var orderId = 1;
        var order = new Order { Id = orderId };

        _orderRepositoryMock.Setup(o => o.FirstOrDefaultAsync(It.IsAny<EntityByIdSpec<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _orderRepositoryMock.Setup(o => o.RemoveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _orderService.RemoveOrderAsync(orderId, TestContext.Current.CancellationToken);

        // Assert
        _orderRepositoryMock.Verify(o => o.FirstOrDefaultAsync(It.IsAny<EntityByIdSpec<Order>>(), It.IsAny<CancellationToken>()));
        _orderRepositoryMock.Verify(o => o.RemoveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task RemoveOrderAsync_OrderNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var orderId = 1;

        _orderRepositoryMock.Setup(o => o.FirstOrDefaultAsync(It.IsAny<EntityByIdSpec<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var ex = await Assert.ThrowsAsync<OrderNotFoundException>(() => _orderService.RemoveOrderAsync(orderId, TestContext.Current.CancellationToken));

        // Assert
        _orderRepositoryMock.Verify(o => o.FirstOrDefaultAsync(It.IsAny<EntityByIdSpec<Order>>(), It.IsAny<CancellationToken>()));
        Assert.Equal("Order not found", ex.Message);
    }
}