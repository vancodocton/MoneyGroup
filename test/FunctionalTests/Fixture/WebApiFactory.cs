using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MoneyGroup.WebApi.Authorizations;

namespace MoneyGroup.FunctionalTests.Fixture;

public sealed class WebApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddUserSecrets<WebApiFactory>();
        });

        builder.ConfigureLogging(logging =>
        {
            logging.AddFilter("MoneyGroup", LogLevel.Debug);
        });

        // Satisfy both the authentication gate (DenyAnonymousAuthorizationRequirement, added by
        // RequireAuthenticatedUser) and the app's own requirement, so requests run unauthenticated.
        // Neither the built-in handler nor DenyUnauthorizedUserHandler calls context.Fail(), so
        // adding a handler that succeeds is enough; the real registrations can stay in place.
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IAuthorizationHandler, BypassDenyUnauthorizedUserRequirementHandler>();
            services.AddSingleton<IAuthorizationHandler, BypassDenyAnonymousAuthorizationRequirementHandler>();
        });
    }

    private sealed class BypassDenyUnauthorizedUserRequirementHandler : AuthorizationHandler<DenyUnauthorizedUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DenyUnauthorizedUserRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    private sealed class BypassDenyAnonymousAuthorizationRequirementHandler : AuthorizationHandler<DenyAnonymousAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DenyAnonymousAuthorizationRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
