﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Models.Paginations;

using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace MoneyGroup.WebApi.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Order")
            .AddFluentValidationAutoValidation()
            .RequireAuthorization()
            .WithTags("Order");

        group.MapGet("/", GetOrdersAsync)
        .WithName("GetOrders")
        .WithOpenApi();

        group.MapGet("/{id:int}", GetOrderByIdAsync)
        .WithName("GetOrderById")
        .WithOpenApi();

        group.MapPost("/", CreateOrderAsync)
        .WithName("CreateOrder")
        .WithOpenApi();

        group.MapDelete("/{id:int}", DeleteOrderAsync)
        .WithName("DeleteOrder")
        .WithOpenApi();
    }

    private static async Task<Results<Ok<PaginatedModel<OrderDetailedDto>>, ValidationProblem>> GetOrdersAsync([AsParameters] OrderPaginatedRequest request, [FromServices] IOrderService orderService)
    {
        var orders = await orderService.GetOrdersByPageAsync(request);
        return TypedResults.Ok(orders);
    }

    private static async Task<Results<CreatedAtRoute<OrderDto>, ValidationProblem>> CreateOrderAsync(OrderDto input, IOrderService orderService)
    {
        await orderService.CreateOrderAsync(input);
        return TypedResults.CreatedAtRoute(input, "GetOrderById", new { id = input.Id });
    }

    private static async Task<Results<NoContent, NotFound>> DeleteOrderAsync(int id, IOrderService orderService, CancellationToken cancellationToken)
    {
        var result = await orderService.RemoveOrderAsync(id, cancellationToken);
        return !result
            ? TypedResults.NotFound()
            : TypedResults.NoContent();
    }

    public static async Task<Results<Ok<OrderDetailedDto>, NotFound>> GetOrderByIdAsync(int id, IOrderService orderService)
    {
        var order = await orderService.GetOrderByIdAsync(id);
        return order == null
            ? TypedResults.NotFound()
            : TypedResults.Ok(order);
    }
}
