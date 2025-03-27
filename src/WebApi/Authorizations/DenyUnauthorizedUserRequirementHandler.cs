using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Specifications;

namespace MoneyGroup.WebApi.Authorizations;

public class DenyUnauthorizedUserRequirementHandler
    : AuthorizationHandler<DenyUnauthorizedUserRequirement>
{
    private readonly ILogger<DenyUnauthorizedUserRequirementHandler> _logger;
    private readonly IUserRepository _userRepository;

    public DenyUnauthorizedUserRequirementHandler(ILogger<DenyUnauthorizedUserRequirementHandler> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DenyUnauthorizedUserRequirement requirement)
    {
        if (context.User == null)
            return;

        string? userEmail = null;
        foreach (var claim in context.User.Claims)
        {
            if (string.Equals(claim.Type, ClaimTypes.Email, StringComparison.OrdinalIgnoreCase))
            {
                userEmail = claim.Value;
                break;
            }
        }
        if (userEmail == null || string.IsNullOrWhiteSpace(userEmail))
        {
            _logger.LogDebug("Require `{JwtRegisteredClaimName}` claim", JwtRegisteredClaimNames.Email);
            return;
        }
        _logger.LogDebug("User email: `{UserEmail}`", userEmail);

        string? emailVerified = null;
        foreach (var claim in context.User.Claims)
        {
            if (string.Equals(claim.Type, JwtRegisteredClaimNames.EmailVerified, StringComparison.OrdinalIgnoreCase))
            {
                emailVerified = claim.Value;
                break;
            }
        }
        if (emailVerified == null)
        {
            _logger.LogDebug("Require `{JwtRegisteredClaimName}` claim", JwtRegisteredClaimNames.EmailVerified);
            return;
        }

        var user = await _userRepository.FirstOrDefaultAsync(new UserByEmailSpec(userEmail));
        if (user is null)
        {
            _logger.LogDebug("User with email `{UserEmail}` not existed", userEmail);
            return;
        }

        context.Succeed(requirement);
    }
}