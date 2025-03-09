using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace MoneyGroup.WebApi;

public class WebApiClientFactory
{
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly HttpClient _httpClient;

    public WebApiClientFactory(HttpClient httpClient)
    {
        _authenticationProvider = new AnonymousAuthenticationProvider();
        _httpClient = httpClient;
    }

    public WebApiClient GetClient()
    {
        return new WebApiClient(new HttpClientRequestAdapter(_authenticationProvider, httpClient: _httpClient));
    }
}
