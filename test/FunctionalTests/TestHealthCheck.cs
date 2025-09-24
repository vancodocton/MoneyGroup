using System.Net.Http.Json;

using MoneyGroup.FunctionalTests.Fixture;

namespace MoneyGroup.FunctionalTests;
public class TestHealthCheck
    : IClassFixture<DistributedAppFactory>
{
    private readonly DistributedAppFactory _factory;

    public TestHealthCheck(DistributedAppFactory webApiFactory)
    {
        _factory = webApiFactory;
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateWebApiClient();

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
        var client = _factory.CreateWebApiClient();

        // Act
        var response = await client.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        var oas = await response.Content.ReadFromJsonAsync<dynamic>(TestContext.Current.CancellationToken);
        Assert.NotNull(oas);
    }
}
