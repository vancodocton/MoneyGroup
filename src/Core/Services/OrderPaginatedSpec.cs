using Ardalis.Specification;

using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Specifications;

namespace MoneyGroup.Core.Services;

public class OrderPaginatedSpec : BasePaginatedSpecification<Order>
{
    public OrderPaginatedSpec(OrderPaginatedOptions options)
        : base(options)
    {
        if (options.BuyerId != null)
            Query.Where(o => o.BuyerId == options.BuyerId);

        if (options.ParticipantId != null)
        {
            Query.Where(o => o.Participants.Any(p => p.ParticipantId == options.ParticipantId));
        }

        if (options.TotalMax != null)
        {
            Query.Where(o => o.Total <= options.TotalMax);
        }

        if (options.TotalMin != null)
        {
            Query.Where(o => o.Total >= options.TotalMin);
        }

        Query.OrderByDescending(c => c.Id);
    }
}