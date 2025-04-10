using System.ComponentModel.DataAnnotations;

namespace MoneyGroup.AgentApp.Options;

public sealed class AzureOpenAIOptions
{
    public const string SectionName = "AzureOpenAI";

    [Required]
    public string ChatDeploymentName { get; set; } = null!;

    [Required]
    public string Endpoint { get; set; } = null!;

    [Required]
    public string ApiKey { get; set; } = null!;
}