using System.Net.Http.Headers;

using Duende.IdentityModel.Client;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        var configuration = host.Services.GetRequiredService<IConfiguration>();

        using var client = new HttpClient();

        _accessToken = GetAccessTokenAsync(client, configuration).ConfigureAwait(false).GetAwaiter().GetResult();

        return host;
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);

        if (_accessToken is not null)
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

    }
}
