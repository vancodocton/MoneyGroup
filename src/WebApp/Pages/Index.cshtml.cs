using Microsoft.AspNetCore.Mvc.RazorPages;

using MoneyGroup.WebApi;
using MoneyGroup.WebApi.Models;

namespace MoneyGroup.WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly WebApiClient _webApiClient;

    public IndexModel(ILogger<IndexModel> logger, WebApiClient webApiClient)
    {
        _logger = logger;
        _webApiClient = webApiClient;
    }

    public PaginationModelOfOrderDetailedDto? OrdersPaginatedModel { get; private set; }

    public async Task OnGet()
    {
        _logger.LogInformation("OnGet called");
        OrdersPaginatedModel = await _webApiClient.Api.Order.GetAsync() ?? throw new InvalidOperationException();
    }
}
