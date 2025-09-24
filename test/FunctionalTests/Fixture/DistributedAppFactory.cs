using System.Net.Http.Headers;

using Aspire.Hosting;

using Aspire.Hosting.Testing;

using Duende.IdentityModel.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MoneyGroup.FunctionalTests.Fixture;

public class DistributedAppFactory
    : DistributedApplicationFactory
{
    public DistributedAppFactory()
        : base(typeof(Projects.MoneyGroup_AppHost))
    {
    }

    private string? _accessToken;

    private static async Task<string?> GetAccessTokenAsync(HttpClient client, IConfiguration configuration)
    {
        var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
        {
            Address = "https://oauth2.googleapis.com/token",
            ClientId = configuration["Test:Google:ClientId"]!,
            ClientSecret = configuration["Test:Google:ClientSecret"]!,
            RefreshToken = configuration["Test:Google:RefreshToken"]!,
        }).ConfigureAwait(false);
        return tokenResponse.IsError ? throw new InvalidOperationException(tokenResponse.Error) : tokenResponse.IdentityToken;
    }

    protected override void OnBuilderCreated(DistributedApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Configuration.AddUserSecrets<DistributedAppFactory>();

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

        using var client = new HttpClient();

        _accessToken = GetAccessTokenAsync(client, applicationBuilder.Configuration).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public HttpClient CreateWebApiClient()
    {
        var client = CreateHttpClient("moneygroup-webapi");

        if (_accessToken is not null)
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        return client;
    }
}
