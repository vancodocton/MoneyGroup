﻿using System.Linq.Expressions;

using MoneyGroup.Core.Models;
using MoneyGroup.Core.Services.Specifications;

namespace MoneyGroup.Core.Abstractions;

public interface IRepository<TEntity>
{
    public Task<PaginationModel<TEntity>> GetByPageAsync(IPaginatedSpecification<TEntity> specification);

    public Task<PaginationModel<TResult>> GetByPageAsync<TResult>(IPaginatedSpecification<TEntity> specification);

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    public Task<TResult> AddAsync<TResult>(TResult dto, CancellationToken cancellationToken = default);

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    public Task<TResult> UpdateAsync<TResult>(TResult dto, CancellationToken cancellationToken = default);

    public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}