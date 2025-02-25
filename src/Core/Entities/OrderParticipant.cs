namespace MoneyGroup.Core.Entities;

public class OrderParticipant
{
    public int ParticipantId { get; set; }

    public int OrderId { get; set; }

    public User Participant { get; set; } = null!;

    public Order Order { get; set; } = null!;
}