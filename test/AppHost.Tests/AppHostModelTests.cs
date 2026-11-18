namespace AppHost.Tests;

public class AppHostModelTests
{
    [Fact]
    public async Task WebApiHasHealthCheckAndClientAppWaitsForIt()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.MoneyGroup_AppHost>(cancellationToken);

        // Act
        var webapi = builder.Resources.Single(r => r.Name == "moneygroup-webapi");
        var clientapp = builder.Resources.Single(r => r.Name == "moneygroup-clientapp");

        // Assert
        Assert.Contains(webapi.Annotations, a => a is HealthCheckAnnotation);
        Assert.Contains(clientapp.Annotations,
            a => a is WaitAnnotation wait && wait.Resource.Name == "moneygroup-webapi");
    }
}
