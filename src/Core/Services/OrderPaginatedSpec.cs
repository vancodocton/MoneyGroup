using Ardalis.Specification;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Services.Specifications;

namespace MoneyGroup.Core.Services;

public class OrderPaginatedSpec : BasePaginatedSpecification<Order>
{
    public OrderPaginatedSpec(IPaginationOptions options)
        : base(options)
    {
        Query.OrderByDescending(c => c.Id);
    }
}
