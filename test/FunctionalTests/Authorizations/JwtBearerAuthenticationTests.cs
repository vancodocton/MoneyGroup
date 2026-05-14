using System.Net;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using MoneyGroup.FunctionalTests.Fixture;

namespace MoneyGroup.FunctionalTests.Authorizations;

public class JwtBearerAuthenticationTests
    : IClassFixture<WebApplicationFactory<Program>>
    , IClassFixture<WebApiAccessTokenFixture>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly WebApiAccessTokenFixture _apiAccessTokenFixture;

    public JwtBearerAuthenticationTests(WebApplicationFactory<Program> factory, WebApiAccessTokenFixture apiAccessTokenFixture)
    {
        _factory = factory;
        _apiAccessTokenFixture = apiAccessTokenFixture;
    }

    [Fact]
    public async Task ProtectedEndpoint_WithJwtBearerToken_ReturnsOk()
    {
        // Arrange
        var authenticatedUri = "/authenticated-endpoint";
        var client = _factory.WithWebHostBuilder(b =>
        {
            b.Configure(app =>
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet(authenticatedUri, () => "Authenticated")
                        .RequireAuthorization();
                });
            });
        }).CreateClient();

        _apiAccessTokenFixture.ConfigureClient(client);

        // Act
        var response = await client.GetAsync(authenticatedUri, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GivenProtectedEndpoint_WhenNoJwtBearerToken_ReturnsUnauthorized()
    {
        // Arrange
        var authenticatedUri = "/authenticated-endpoint";
        _factory.WithWebHostBuilder(b =>
        {
            b.Configure(app =>
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet(authenticatedUri, () => "Authenticated")
                        .RequireAuthorization();
                });
            });
        });
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(authenticatedUri, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
