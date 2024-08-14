using System.Linq.Expressions;

namespace MoneyGroup.Core.Abstractions;

public interface IRepository<TEntity>
{
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    public Task<TResult> AddAsync<TResult>(TResult dto, CancellationToken cancellationToken = default);

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    public Task<TResult> UpdateAsync<TResult>(TResult dto, CancellationToken cancellationToken = default);

    public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}