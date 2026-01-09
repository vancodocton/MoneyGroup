using Aspire.Hosting;
using Aspire.Hosting.Testing;

using Microsoft.Extensions.Logging;

namespace AppHost.Tests.Fixtures;

public sealed class AppHostFactory()
    : DistributedApplicationFactory(typeof(Projects.MoneyGroup_AppHost))
{
    protected override void OnBuilderCreated(DistributedApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            // Override the logging filters from the app's configuration
            logging.AddFilter(applicationBuilder.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
            // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
        });

        applicationBuilder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
    }
}
