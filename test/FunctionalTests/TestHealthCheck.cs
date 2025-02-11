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
}