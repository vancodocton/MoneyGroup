using System.ComponentModel;
using System.Text.Json;

using Microsoft.SemanticKernel;

using Newtonsoft.Json;

namespace MoneyGroup.AgentApp.Plugins;

internal class OrderPlugin(ILogger<OrderPlugin> logger, IMoneyGroup_WebApiClient webApiClient)
{
    private readonly ILogger<OrderPlugin> _logger = logger;
    private readonly IMoneyGroup_WebApiClient _webApiClient = webApiClient;

    [KernelFunction, Description("Create new order and return created order")]
    public async Task<OrderDto> SetAlarm([Description("DTO of order to create")] OrderDto orderDto, CancellationToken cancellation)
    {
        var createdOrder = await _webApiClient.CreateOrderAsync(orderDto, cancellation);

        _logger.LogInformation("Created order: {@Order}", JsonConvert.SerializeObject(createdOrder));

        return createdOrder;
    }
}