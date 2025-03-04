using Ardalis.Specification;

using MoneyGroup.Core.Abstractions;

namespace MoneyGroup.Core.Services.Specifications;

public interface IPaginatedSpecification<TEntity> : ISpecification<TEntity>
{
    public IPaginatedOptions PaginatedOptions { get; }
}
