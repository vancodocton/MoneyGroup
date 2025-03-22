using Ardalis.Specification;

using MoneyGroup.Core.Abstractions;

namespace MoneyGroup.Core.Specifications;

public class BasePaginatedSpecification<TRequest, TEntity> : Specification<TEntity>
    , IPaginatedSpecification<TRequest, TEntity>
    where TRequest : IPaginatedOptions
{
    public TRequest PaginatedOptions { get; }

    public BasePaginatedSpecification(TRequest options)
    {
        PaginatedOptions = options;

        Query.Skip((options.Page - 1) * options.Size).Take(options.Size);
    }
}