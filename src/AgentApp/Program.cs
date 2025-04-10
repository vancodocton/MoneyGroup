using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

using MoneyGroup.AgentApp;
using MoneyGroup.AgentApp.Options;
using MoneyGroup.AgentApp.Plugins;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddOptions<AzureOpenAIOptions>()
    .Bind(builder.Configuration.GetSection(AzureOpenAIOptions.SectionName))
    .ValidateOnStart();

// Chat completion service that kernels will use
builder.Services.AddSingleton<IChatCompletionService>(sp =>
{
    AzureOpenAIOptions azureOpenAIOptions = sp.GetRequiredService<IOptions<AzureOpenAIOptions>>().Value;
    return new AzureOpenAIChatCompletionService(azureOpenAIOptions.ChatDeploymentName, azureOpenAIOptions.Endpoint, azureOpenAIOptions.ApiKey);
});

builder.Services.AddSingleton<OrderPlugin>();

builder.Services.AddTransient(sp =>
{
    // Create a collection of plugins that the kernel will use
    KernelPluginCollection pluginCollection = [];
    var orderPlugin = sp.GetRequiredService<OrderPlugin>();
    pluginCollection.AddFromObject(orderPlugin);

    // When created by the dependency injection container, Semantic Kernel logging is included by default
    return new Kernel(sp, pluginCollection);
});


builder.Services.AddHttpClient<IMoneyGroup_WebApiClient, MoneyGroup_WebApiClient>(options =>
{
    var url = builder.Configuration["Services:WebApi:Url"];
    ArgumentException.ThrowIfNullOrEmpty(url);
    options.BaseAddress = new Uri(url);
});

var host = builder.Build();

await host.RunAsync();