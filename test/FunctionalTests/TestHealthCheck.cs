using System.Net.Http.Json;

using MoneyGroup.FunctionalTests.Fixture;

namespace MoneyGroup.FunctionalTests;
public class TestHealthCheck
    : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _webApiFactory;

    public TestHealthCheck(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory;
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Arrange
        var client = _webApiFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/healthz", TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Equal("Healthy", responseString);
    }

    [Fact]
    public async Task OpenApiSpecification_ReturnsOk()
    {
        // Arrange
        var client = _webApiFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        var oas = await response.Content.ReadFromJsonAsync<dynamic>(TestContext.Current.CancellationToken);
        Assert.NotNull(oas);
    }
}
