namespace MoneyGroup.Core.Entities;

public class Order
    : EntityBase
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Total { get; set; }

    public OrderIssuer Issuer { get; set; } = null!;

    public List<OrderConsumer> Consumers { get; set; } = [];
}