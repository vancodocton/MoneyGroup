using System.Linq.Expressions;

using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Infrastructure.Mapperly;

namespace MoneyGroup.Infrastructure.Data;

public class EfRepository<TEntity>
    : IRepository<TEntity> where TEntity : class
{
    protected readonly ISpecificationEvaluator _evaluator = SpecificationEvaluator.Default;
    protected readonly DbContext _dbContext;
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly IMapper _mapper;

    public EfRepository(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
        _mapper = mapper;
    }

    public async Task<PaginatedModel<TEntity>> GetByPageAsync(IPaginatedSpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        var total = await _evaluator.GetQuery(query, specification, evaluateCriteriaOnly: true)
            .CountAsync(cancellationToken);
        var items = await _evaluator.GetQuery(query, specification, evaluateCriteriaOnly: false)
            .ToListAsync(cancellationToken);

        var model = new PaginatedModel<TEntity>()
        {
            Page = specification.PaginatedOptions.Page,
            Count = items.Count,
            Total = total,
            Items = items,
        };

        return model;
    }

    public async Task<PaginatedModel<TResult>> GetByPageAsync<TResult>(IPaginatedSpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        var total = await _evaluator.GetQuery(query, specification, evaluateCriteriaOnly: true)
            .CountAsync(cancellationToken);
        var items = await _evaluator.GetQuery(query, specification, evaluateCriteriaOnly: false)
            .ProjectTo<TResult>(_mapper)
            .ToListAsync(cancellationToken);

        var model = new PaginatedModel<TResult>()
        {
            Page = specification.PaginatedOptions.Page,
            Count = items.Count,
            Total = total,
            Items = items,
        };

        return model;
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .WithSpecification(specification)
            .AnyAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .WithSpecification(specification)
            .CountAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .WithSpecification(specification)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ProjectTo<TResult>(_mapper).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .WithSpecification(specification)
            .ProjectTo<TResult>(_mapper)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .WithSpecification(specification)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }
}
