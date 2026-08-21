using Microsoft.AspNetCore.Http.HttpResults;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.UnitTests.Builders;
using MoneyGroup.WebApi.Endpoints;

using NSubstitute;

namespace MoneyGroup.UnitTests.WebApi.Endpoints;

[Trait("Category", "Unit")]
public class OrderEndpointsTests
{
    private readonly IOrderService _orderService = Substitute.For<IOrderService>();

    [Fact]
    public async Task GivenOrderId_WhenOrderExists_ThenReturnsOk()
    {
        // Arrange
        var order = new OrderDetailedDto { Id = 1, Title = "Order 1" };
        _orderService.GetOrderByIdAsync(1, Arg.Any<CancellationToken>()).Returns(order);

        // Act
        var result = await OrderEndpoints.GetOrderByIdAsync(1, _orderService);

        // Assert
        var ok = Assert.IsType<Ok<OrderDetailedDto>>(result.Result);
        Assert.Same(order, ok.Value);
    }

    [Fact]
    public async Task GivenOrderId_WhenOrderMissing_ThenReturnsNotFound()
    {
        // Arrange
        _orderService.GetOrderByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((OrderDetailedDto?)null);

        // Act
        var result = await OrderEndpoints.GetOrderByIdAsync(int.MaxValue, _orderService);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task GivenOrderId_WhenOrderRemoved_ThenReturnsNoContent()
    {
        // Arrange
        _orderService.RemoveOrderAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await OrderEndpoints.DeleteOrderAsync(1, _orderService, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NoContent>(result.Result);
    }

    [Fact]
    public async Task GivenOrderId_WhenNothingRemoved_ThenReturnsNotFound()
    {
        // Arrange
        _orderService.RemoveOrderAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await OrderEndpoints.DeleteOrderAsync(
            int.MaxValue, _orderService, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task GivenOrder_WhenCreated_ThenReturnsCreatedAtRouteWithNewId()
    {
        // Arrange
        var input = OrderDtoBuilder.Valid().WithBuyer(1).WithParticipants(1, 2).Build();
        _orderService
            .When(s => s.CreateOrderAsync(input, Arg.Any<CancellationToken>()))
            .Do(_ => input.Id = 99);

        // Act
        var result = await OrderEndpoints.CreateOrderAsync(input, _orderService);

        // Assert
        var created = Assert.IsType<CreatedAtRoute<OrderDto>>(result.Result);
        Assert.Same(input, created.Value);
        Assert.Equal("GetOrderById", created.RouteName);
        Assert.Equal(99, created.RouteValues["id"]);
    }

    [Fact]
    public async Task GivenPagingRequest_WhenCalled_ThenReturnsOkWithServiceResult()
    {
        // Arrange
        var request = new OrderPaginatedRequest(null, null, null, null, 1, 10);
        var expected = new PaginatedModel<OrderDetailedDto>
        {
            Page = 1,
            Count = 0,
            Total = 0,
            Items = [],
        };
        _orderService.GetOrdersByPageAsync(request).Returns(expected);

        // Act
        var result = await OrderEndpoints.GetOrdersAsync(request, _orderService);

        // Assert
        var ok = Assert.IsType<Ok<PaginatedModel<OrderDetailedDto>>>(result.Result);
        Assert.Same(expected, ok.Value);
    }
}
