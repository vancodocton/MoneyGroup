namespace MoneyGroup.Core.Models.Orders;

public class OrderDto
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Total { get; set; }

    public int BuyerId { get; set; }

    public IEnumerable<ParticipantDto> Participants { get; set; } = null!;
}

public class OrderDetailedDto
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Total { get; set; }

    public int BuyerId { get; set; }

    public string BuyerName { get; set; } = null!;

    public IEnumerable<ParticipantDetailedDto> Participants { get; set; } = null!;
}
