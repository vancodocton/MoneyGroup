using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MoneyGroup.Core.Models.Users;
using MoneyGroup.WebApi.Authorizations;
using MoneyGroup.WebApi.Features;

namespace MoneyGroup.FunctionalTests.Fixture;

public sealed class WebApiFactory : WebApplicationFactory<Program>
{
    private const string TestAuthScheme = "TestScheme";

    /// <summary>
    /// Gets or sets the current user feature to be set during authorization.
    /// </summary>
    public UserDto CurrentUser { get; set; } = new()
    {
        Id = -1,
        Name = "Test User",
        Email = "user@example.com",
    };

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

        builder.ConfigureServices(services =>
        {
            // Replace DenyUnauthorizedUserHandler with a bypass handler for testing
            services.RemoveAll<IAuthorizationHandler>();
            services.AddSingleton(this);
            services.AddTransient<IAuthorizationHandler, BypassDenyUnauthorizedUserHandler>();

            // Add test authentication scheme
            services.AddAuthentication(TestAuthScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthScheme, options => { });

            // Override default authorization policy to use test scheme
            services.AddAuthorizationBuilder()
                .SetDefaultPolicy(new AuthorizationPolicyBuilder(TestAuthScheme)
                    .RequireAuthenticatedUser()
                    .RequireAuthorizedUser()
                    .Build());
        });
    }

    private sealed class BypassDenyUnauthorizedUserHandler
        : AuthorizationHandler<DenyUnauthorizedUserRequirement>
    {
        private readonly WebApiFactory _factory;

        public BypassDenyUnauthorizedUserHandler(WebApiFactory factory)
        {
            _factory = factory;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            DenyUnauthorizedUserRequirement requirement)
        {
            context.Succeed(requirement);

            if (_factory.CurrentUser != null &&
                context.Resource is HttpContext httpContext)
            {
                httpContext.Features.Set<ICurrentUserFeature>(new CurrentUserFeature()
                {
                    User = _factory.CurrentUser,
                });
            }

            return Task.CompletedTask;
        }
    }

    private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly WebApiFactory _webApiFactory;

        public TestAuthHandler(
            WebApiFactory webApiFactory,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            System.Text.Encodings.Web.UrlEncoder encoder)
            : base(options, logger, encoder)
        {
            _webApiFactory = webApiFactory;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var user = _webApiFactory.CurrentUser;
            if (user == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("No user set for testing."));
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.Name),
            };

            var identity = new ClaimsIdentity(claims, TestAuthScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, TestAuthScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
