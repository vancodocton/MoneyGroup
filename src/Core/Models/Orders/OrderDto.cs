namespace MoneyGroup.Core.Models.Orders;

public class OrderDto
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Total { get; set; }

    public int IssuerId { get; set; }

    public IEnumerable<ConsumerDto> Consumers { get; set; } = null!;
}