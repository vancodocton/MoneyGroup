using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

using MoneyGroup.Core.Abstractions;

namespace MoneyGroup.WebApi.Authorizations;

public class DenyUnauthorizedUserHandler
    : AuthorizationHandler<DenyUnauthorizedUserRequirement>
{
    private readonly ILogger<DenyUnauthorizedUserHandler> _logger;
    private readonly IUserService _userService;

    public DenyUnauthorizedUserHandler(ILogger<DenyUnauthorizedUserHandler> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DenyUnauthorizedUserRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var userEmail = context.User.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null || string.IsNullOrWhiteSpace(userEmail))
        {
            _logger.LogDebug("Require `{JwtRegisteredClaimName}` claim", JwtRegisteredClaimNames.Email);
            return;
        }
        _logger.LogDebug("User email: `{UserEmail}`", userEmail);

        var emailVerified = context.User.FindFirstValue(JwtRegisteredClaimNames.EmailVerified);
        if (emailVerified == null)
        {
            _logger.LogDebug("Require `{JwtRegisteredClaimName}` claim", JwtRegisteredClaimNames.EmailVerified);
            return;
        }

        var user = await _userService.GetUserByEmailAsync(userEmail);
        if (user is null)
        {
            _logger.LogDebug("User with email `{UserEmail}` not existed", userEmail);
            return;
        }

        context.Succeed(requirement);
    }
}
