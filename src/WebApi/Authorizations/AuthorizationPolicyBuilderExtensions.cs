using Microsoft.AspNetCore.Authorization;

namespace MoneyGroup.WebApi.Authorizations;

public static class AuthorizationPolicyBuilderExtensions
{
    private static readonly DenyUnauthorizedUserRequirement DenyUnauthorizedUserRequirement = new();

    public static AuthorizationPolicyBuilder RequireAuthorizedUser(this AuthorizationPolicyBuilder builder)
    {
        builder.Requirements.Add(DenyUnauthorizedUserRequirement);
        return builder;
    }
}