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
        .WithName("GetUsers");

        group.MapGet("/{id:int}", GetUserByIdAsync)
        .WithName("GetUserById");

        group.MapGet("/my", GetExecutingUser)
        .WithName("GetExecutingUser");
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

    public static Ok<UserDto> GetExecutingUser(HttpContext httpContext)
    {
        var feature = httpContext.Features.Get<ICurrentUserFeature>();
        ArgumentNullException.ThrowIfNull(feature?.User);
        return TypedResults.Ok(feature.User);
    }
}
