namespace MoneyGroup.Core.Entities;

public class OrderConsumer
{
    public int ConsumerId { get; set; }

    public int OrderId { get; set; }

    public User Consumer { get; set; } = null!;

    public Order Order { get; set; } = null!;
}