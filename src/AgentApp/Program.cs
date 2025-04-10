using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
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

// Chat completion service that kernels will use
builder.Services.AddKeyedSingleton<Agent>("MoneyGroupAgent", (sp, _) =>
{
    var kernel = sp.GetRequiredService<Kernel>();

    PromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    };

    ChatCompletionAgent agent = new()
    {
        Name = "MoneyGroupAgent",
        Instructions = """"
        You are an agent for MoneyGroup application . You are able to access the MoneyGroup API via plugins.

        You can assist user to:
        1. Create new order, and then show user created user in a human-readable format that must include order Id. A order is a record of a purchase that sharing among a group of participants.

        Currently, you know that:
        *`1` is id of user `Truong`,
        * `2` is id of user `Duc`.
        * Current user is `Truong`.
        """",
        Kernel = kernel,
        Arguments = new KernelArguments(openAIPromptExecutionSettings),
    };

    return agent;
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