using Ardalis.Specification;

using MoneyGroup.Core.Abstractions;

namespace MoneyGroup.Core.Services.Specifications;

public class BasePaginatedSpecification<TEntity> : Specification<TEntity>
    , IPaginatedSpecification<TEntity>
{
    public IPaginationOptions PaginatedOptions { get; }

    public BasePaginatedSpecification(IPaginationOptions options)
    {
        PaginatedOptions = options;

        Query.Skip((options.Page - 1) * options.Size).Take(options.Size);
    }
}
