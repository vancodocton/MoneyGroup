using System.Security.Claims;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.WebApi.Features;

using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace MoneyGroup.WebApi.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/User")
            .AddFluentValidationAutoValidation()
            .RequireAuthorization()
            .WithTags("User");

        group.MapGet("/", GetUsersAsync)
        .WithName("GetUsers")
        .WithOpenApi();

        group.MapGet("/{id:int}", GetUserByIdAsync)
        .WithName("GetUserById")
        .WithOpenApi();

        group.MapGet("/my", GetExecutingUser)
        .WithName("GetExecutingUser")
        .WithOpenApi();
    }

    private static async Task<Results<Ok<PaginatedModel<UserDto>>, ValidationProblem>> GetUsersAsync([AsParameters] UserPaginatedRequest request, [FromServices] IUserService userService)
    {
        var users = await userService.GetUsersByPageAsync(request);
        return TypedResults.Ok(users);
    }

    public static async Task<Results<Ok<UserDto>, NotFound>> GetUserByIdAsync(int id, [FromServices] IUserService userService, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByIdAsync(id, cancellationToken);
        return user == null
            ? TypedResults.NotFound()
            : TypedResults.Ok(user);
    }

    public static Results<Ok<UserDto>, UnauthorizedHttpResult> GetExecutingUser(HttpContext httpContext)
    {
        var user = httpContext.Features.Get<ICurrentUserFeature>()?.User;
        return user == null
            ? TypedResults.Unauthorized()
            : TypedResults.Ok(user);
    }
}
