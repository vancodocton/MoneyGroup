using MoneyGroup.Core.Models.Paginations;

namespace MoneyGroup.Core.Models.Orders;

public class OrderPaginatedOptions
    : PaginatedOptions
{
    public OrderPaginatedOptions(int? buyerId, int? participantId, decimal? totalMax, decimal? totalMin, int page, int size)
        : base(page, size)
    {
        BuyerId = buyerId;
        ParticipantId = participantId;
        TotalMax = totalMax;
        TotalMin = totalMin;
    }

    public int? BuyerId { get; }

    public int? ParticipantId { get; }

    public decimal? TotalMax { get; }

    public decimal? TotalMin { get; }
}