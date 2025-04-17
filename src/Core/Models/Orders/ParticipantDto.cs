using System.Text.Json.Serialization;

namespace MoneyGroup.Core.Models.Orders;

public class ParticipantDto
{
    [JsonPropertyName("id")]
    public int ParticipantId { get; set; }
}

public class ParticipantDetailedDto
{
    [JsonPropertyName("id")]
    public int ParticipantId { get; set; }

    [JsonPropertyName("name")]
    public string ParticipantName { get; set; } = null!;
}
