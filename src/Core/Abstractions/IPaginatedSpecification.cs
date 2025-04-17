using Ardalis.Specification;

namespace MoneyGroup.Core.Abstractions;

public interface IPaginatedSpecification<TEntity> : ISpecification<TEntity>
{
    public IPaginatedOptions PaginatedOptions { get; }
}
