using Ardalis.Specification;

namespace MoneyGroup.Core.Abstractions;

public interface IPaginatedSpecification<TRequest, TEntity> : ISpecification<TEntity>
    where TRequest : IPaginatedOptions
{
    public TRequest PaginatedOptions { get; }
}