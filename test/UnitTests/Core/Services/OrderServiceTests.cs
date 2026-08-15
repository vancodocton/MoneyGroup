using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Exceptions;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Services;
using MoneyGroup.Core.Specifications;
using MoneyGroup.UnitTests.Builders;

using NSubstitute;

namespace MoneyGroup.UnitTests.Core.Services;

[Trait("Category", "Unit")]
public class OrderServiceTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderService = new OrderService(_orderRepository, _userRepository);
    }

    [Fact]
    public async Task GivenOrderId_WhenOrderExists_ThenReturnsOrder()
    {
        // Arrange
        var id = 1;
        var orderDto = new OrderDetailedDto { Id = id };
        var cancellationToken = TestContext.Current.CancellationToken;

        _orderRepository
            .FirstOrDefaultAsync<OrderDetailedDto>(Arg.Any<EntityByIdSpec<Order>>(), cancellationToken)
            .Returns(orderDto);

        // Act
        var result = await _orderService.GetOrderByIdAsync(id, cancellationToken);

        // Assert
        await _orderRepository.Received(1)
            .FirstOrDefaultAsync<OrderDetailedDto>(Arg.Any<EntityByIdSpec<Order>>(), cancellationToken);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GivenOrderId_WhenOrderMissing_ThenReturnsNull()
    {
        // Arrange
        var invalidId = -1;
        var cancellationToken = TestContext.Current.CancellationToken;

        _orderRepository
            .FirstOrDefaultAsync<OrderDetailedDto>(Arg.Any<EntityByIdSpec<Order>>(), cancellationToken)
            .Returns((OrderDetailedDto?)null);

        // Act
        var result = await _orderService.GetOrderByIdAsync(invalidId, cancellationToken);

        // Assert
        await _orderRepository.Received(1)
            .FirstOrDefaultAsync<OrderDetailedDto>(Arg.Any<EntityByIdSpec<Order>>(), cancellationToken);
        Assert.Null(result);
    }

    [Fact]
    public async Task GivenOrder_WhenBuyerAndParticipantsExist_ThenAddsOrder()
    {
        // Arrange
        var newOrderId = 1;
        var model = OrderDtoBuilder.Valid().WithBuyer(1).WithParticipants(2, 3).Build();

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
        await _orderRepository.Received(1).AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>());
        Assert.Equal(newOrderId, model.Id);
    }

    [Fact]
    public async Task GivenOrder_WhenNoParticipants_ThenAddsOrderWithoutCountingParticipants()
    {
        // Arrange
        var newOrderId = 1;
        var model = OrderDtoBuilder.Valid().WithBuyer(1).WithNoParticipants().Build();

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
        await _orderRepository.Received(1).AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>());
        Assert.Equal(newOrderId, model.Id);
    }

    [Fact]
    public async Task GivenOrder_WhenBuyerMissing_ThenThrowsBuyerNotFound()
    {
        // Arrange
        var model = OrderDtoBuilder.Valid().WithBuyer(-1).WithParticipants(2, 3).Build();

        _userRepository.AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var ex = await Assert.ThrowsAsync<BuyerNotFoundException>(
            () => _orderService.CreateOrderAsync(model, TestContext.Current.CancellationToken));

        // Assert
        await _userRepository.Received(1).AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>());
        await _orderRepository.DidNotReceive().AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>());
        Assert.Equal("Buyer not found", ex.Message);
    }

    [Fact]
    public async Task GivenOrder_WhenAnyParticipantMissing_ThenThrowsParticipantNotFound()
    {
        // Arrange
        var model = OrderDtoBuilder.Valid().WithBuyer(1).WithParticipants(2, -1).Build();

        _userRepository.AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(true);
        _userRepository.CountAsync(Arg.Any<EntityByIdsSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(1); // only 1 of the 2 requested participants exists

        // Act
        var ex = await Assert.ThrowsAsync<ParticipantNotFoundException>(
            () => _orderService.CreateOrderAsync(model, TestContext.Current.CancellationToken));

        // Assert
        await _userRepository.Received(1).AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>());
        await _userRepository.Received(1).CountAsync(Arg.Any<EntityByIdsSpec<User>>(), Arg.Any<CancellationToken>());
        await _orderRepository.DidNotReceive().AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>());
        Assert.Equal("Participant not found", ex.Message);
    }

    [Fact]
    public async Task GivenOrderId_WhenOrderExists_ThenRemovesOrderAndReturnsTrue()
    {
        // Arrange
        var orderId = 1;
        var order = new Order { Id = orderId };

        _orderRepository.FirstOrDefaultAsync(Arg.Any<EntityByIdSpec<Order>>(), Arg.Any<CancellationToken>())
            .Returns(order);

        // Act
        var result = await _orderService.RemoveOrderAsync(orderId, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        await _orderRepository.Received(1).FirstOrDefaultAsync(Arg.Any<EntityByIdSpec<Order>>(), Arg.Any<CancellationToken>());
        await _orderRepository.Received(1).RemoveAsync(order, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenOrderId_WhenOrderMissing_ThenReturnsFalse()
    {
        // Arrange
        var orderId = 1;

        _orderRepository.FirstOrDefaultAsync(Arg.Any<EntityByIdSpec<Order>>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        // Act
        var result = await _orderService.RemoveOrderAsync(orderId, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
        await _orderRepository.Received(1).FirstOrDefaultAsync(Arg.Any<EntityByIdSpec<Order>>(), Arg.Any<CancellationToken>());
        await _orderRepository.DidNotReceive().RemoveAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
    }
}
