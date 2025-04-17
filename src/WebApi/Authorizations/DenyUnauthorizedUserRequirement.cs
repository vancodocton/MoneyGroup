using Microsoft.AspNetCore.Authorization;

namespace MoneyGroup.WebApi.Authorizations;

public class DenyUnauthorizedUserRequirement
    : IAuthorizationRequirement
{
}
