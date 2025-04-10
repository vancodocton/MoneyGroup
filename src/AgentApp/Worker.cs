using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace MoneyGroup.AgentApp;

public class Worker : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly Kernel _kernel;
    private readonly ILogger<Worker> _logger;

    public Worker(IHostApplicationLifetime hostApplicationLifetime,
        Kernel kernel,
        ILogger<Worker> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _kernel = kernel;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.WhenAny(
            Task.Delay(Timeout.Infinite, stoppingToken),
            Task.Delay(Timeout.Infinite, _hostApplicationLifetime.ApplicationStarted));

        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        string? input;

        await Console.Out.WriteAsync("> ").ConfigureAwait(false);
        while ((input = await Console.In.ReadLineAsync(stoppingToken).ConfigureAwait(false)) is not null)
        {
            _logger.LogDebug("Input: {ChatInput}", input);
            await Console.Out.WriteLineAsync();

            ChatMessageContent chatResult = await chatCompletionService.GetChatMessageContentAsync(input,
                    openAIPromptExecutionSettings, _kernel, stoppingToken).ConfigureAwait(false);

            _logger.LogDebug("Result: {ChatResult}", chatResult);
            await Console.Out.WriteAsync($"\n>>> Result: {chatResult}\n\n> ");
        }

        _hostApplicationLifetime.StopApplication();
    }
}