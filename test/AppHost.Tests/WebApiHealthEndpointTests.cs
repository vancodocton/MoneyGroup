using AppHost.Tests.Fixtures;

namespace AppHost.Tests;

public class WebApiHealthEndpointTests
    : IClassFixture<AppHostFactory>
{
    private readonly AppHostFactory _factory;

    public WebApiHealthEndpointTests(AppHostFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetHealthEndpointReturnsOkStatusCode()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await _factory.StartAsync(cancellationToken);

        // Act
        using var httpClient = _factory.CreateHttpClient("moneygroup-webapi");
        using var response = await httpClient.GetAsync("/health", cancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", responseString);
    }
}
