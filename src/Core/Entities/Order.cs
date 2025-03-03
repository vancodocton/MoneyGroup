namespace MoneyGroup.Core.Entities;

public class Order
    : EntityBase
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Total { get; set; }

    public int BuyerId { get; set; }

    public User Buyer { get; set; } = null!;

    public List<OrderParticipant> Participants { get; set; } = [];
}