using Ardalis.Specification;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Paginations;

namespace MoneyGroup.Core.Specifications;

public class OrderPaginatedSpec : BasePaginatedSpecification<Order>
{
    public OrderPaginatedSpec(IPaginatedOptions options)
        : base(options)
    {
        Query.OrderByDescending(c => c.Id);
    }
}