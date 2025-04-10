using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace MoneyGroup.AgentApp;

public class Worker : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Worker> _logger;

    public Worker(IHostApplicationLifetime hostApplicationLifetime,
        IServiceProvider serviceProvider,
        ILogger<Worker> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.WhenAny(
            Task.Delay(Timeout.Infinite, stoppingToken),
            Task.Delay(Timeout.Infinite, _hostApplicationLifetime.ApplicationStarted));

        var agent = _serviceProvider.GetRequiredKeyedService<Agent>("MoneyGroupAgent");

        AgentThread thread = new ChatHistoryAgentThread();

        string? input;

        await Console.Out.WriteAsync("> ").ConfigureAwait(false);
        while ((input = await Console.In.ReadLineAsync(stoppingToken).ConfigureAwait(false)) is not null)
        {
            _logger.LogDebug("Input: {ChatInput}", input);
            await Console.Out.WriteLineAsync();

            string chatResult = "";
            var message = new ChatMessageContent(AuthorRole.User, input);
            await foreach (var response in agent.InvokeAsync(message, thread, null, stoppingToken))
            {
                _logger.LogInformation("Response: {Response}", response);
                chatResult += response.Message.Content;
            }

            _logger.LogDebug("Result: {ChatResult}", chatResult);
            await Console.Out.WriteAsync($"\n>>> Result: {chatResult}\n\n> ");
        }

        _hostApplicationLifetime.StopApplication();
    }
}