namespace MoneyGroup.Core.Models.Orders;

public class ParticipantDto
{
    public int ParticipantId { get; set; }
}

public class ParticipantDetailedDto
{
    public int ParticipantId { get; set; }

    public string ParticipantName { get; set; } = null!;
}