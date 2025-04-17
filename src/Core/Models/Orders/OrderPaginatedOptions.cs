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

    public virtual int? BuyerId { get; init; }

    public virtual int? ParticipantId { get; init; }

    public virtual decimal? TotalMax { get; init; }

    public virtual decimal? TotalMin { get; init; }
}
