namespace MoneyGroup.Core.Models.Orders;

public class ParticipantDto
{
    public int Id { get; set; }
}

public class ParticipantDetailedDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}