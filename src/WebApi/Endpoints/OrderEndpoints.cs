using Microsoft.AspNetCore.Http.HttpResults;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models.Orders;
namespace MoneyGroup.WebApi.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Order").WithTags("Order");

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

    private static async Task<Results<CreatedAtRoute<OrderDto>, ValidationProblem>> CreateOrderAsync(OrderDto input, IOrderService orderService)
    {
        await orderService.CreateOrderAsync(input);
        return TypedResults.CreatedAtRoute(input, "GetOrderById", new { id = input.Id });
    }

    public static async Task<Results<NoContent, ValidationProblem>> UpdateOrderAsync(int id, OrderDto input, IOrderService orderService)
    {
        await orderService.UpdateOrderAsync(input);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteOrderAsync(int id, IOrderService orderService, CancellationToken cancellationToken)
    {
        await orderService.RemoveOrderAsync(id, cancellationToken);
        return TypedResults.NoContent();
    }

    public static async Task<Results<Ok<OrderDto>, NotFound>> GetOrderByIdAsync(int id, IOrderService orderService)
    {
        var order = await orderService.GetOrderByIdAsync(id);
        return order == null
            ? TypedResults.NotFound()
            : TypedResults.Ok(order);
    }
}