using System.Net.Http.Headers;

using Duende.IdentityModel.Client;

using Microsoft.Extensions.Configuration;

namespace MoneyGroup.FunctionalTests.Fixture;

public sealed class WebApiAccessTokenFixture
    : IAsyncLifetime
{
    public string AccessToken { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        using var client = new HttpClient();
        var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

        var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
        {
            Address = "https://oauth2.googleapis.com/token",
            ClientId = configuration["Test:Google:ClientId"]!,
            ClientSecret = configuration["Test:Google:ClientSecret"]!,
            RefreshToken = configuration["Test:Google:RefreshToken"]!,
        });

        if (tokenResponse.IsError)
        {
            throw new InvalidOperationException("Failed to obtain Google access token for testing.", tokenResponse.Exception);
        }

        if (string.IsNullOrEmpty(tokenResponse.IdentityToken))
        {
            throw new InvalidOperationException("Received empty identity token from Google.");
        }

        AccessToken = tokenResponse.IdentityToken;
    }

    public void ConfigureClient(HttpClient client)
    {

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
