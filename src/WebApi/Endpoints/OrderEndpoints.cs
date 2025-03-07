using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Exceptions;
using MoneyGroup.Core.Models;
using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.WebApi.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Order").WithTags("Order");

        group.MapGet("/", GetOrdersAsync)
        .WithName("GetOrders")
        .WithOpenApi();

        group.MapGet("/{id:int}", GetOrderByIdAsync)
        .WithName("GetOrderById")
        .WithOpenApi();

        group.MapPut("/{id:int}", UpdateOrderAsync)
        .WithName("UpdateOrder")
        .WithOpenApi();

        group.MapPost("/", CreateOrderAsync)
        .WithName("CreateOrder")
        .WithOpenApi();

        group.MapDelete("/{id:int}", DeleteOrderAsync)
        .WithName("DeleteOrder")
        .WithOpenApi();
    }

    private static async Task<Results<Ok<PaginationModel<OrderDto>>, ValidationProblem>> GetOrdersAsync([AsParameters] OrderPaginationRequest request, [FromServices] IOrderService orderService)
    {
        var orders = await orderService.GetOrdersByPageAsync(request);
        return TypedResults.Ok(orders);
    }

    private static async Task<Results<CreatedAtRoute<OrderDto>, ValidationProblem>> CreateOrderAsync(OrderDto input, IOrderService orderService)
    {
        await orderService.CreateOrderAsync(input);
        return TypedResults.CreatedAtRoute(input, "GetOrderById", new { id = input.Id });
    }

    public static async Task<Results<NoContent, ValidationProblem, NotFound>> UpdateOrderAsync(int id, OrderDto input, IOrderService orderService)
    {
        try
        {
            await orderService.UpdateOrderAsync(input);
            return TypedResults.NoContent();
        }
        catch (OrderNotFoundException)
        {
            return TypedResults.NotFound();
        }
    }

    private static async Task<Results<NoContent, NotFound>> DeleteOrderAsync(int id, IOrderService orderService, CancellationToken cancellationToken)
    {
        try
        {
            await orderService.RemoveOrderAsync(id, cancellationToken);
            return TypedResults.NoContent();
        }
        catch (OrderNotFoundException)
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<Ok<OrderDto>, NotFound>> GetOrderByIdAsync(int id, IOrderService orderService)
    {
        var order = await orderService.GetOrderByIdAsync(id);
        return order == null
            ? TypedResults.NotFound()
            : TypedResults.Ok(order);
    }
}