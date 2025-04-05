using System.Linq.Expressions;

using Ardalis.Specification;

using MoneyGroup.Core.Models.Paginations;

namespace MoneyGroup.Core.Abstractions;

public interface IRepository<TEntity>
{
    public Task<PaginatedModel<TEntity>> GetByPageAsync(IPaginatedSpecification<TEntity> specification, CancellationToken cancellationToken = default);

    public Task<PaginatedModel<TResult>> GetByPageAsync<TResult>(IPaginatedSpecification<TEntity> specification, CancellationToken cancellationToken = default);

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    public Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    public Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}