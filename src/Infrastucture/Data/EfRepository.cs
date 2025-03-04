using System.Linq.Expressions;

using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models;
using MoneyGroup.Core.Services.Specifications;

namespace MoneyGroup.Infrastucture.Data;
public class EfRepository<TEntity>
    : IRepository<TEntity> where TEntity : class
{
    protected readonly DbContext _dbContext;
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly IMapper _mapper;

    public EfRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
        _mapper = mapper;
    }

    public async Task<PaginationModel<TEntity>> GetByPageAsync(IPaginatedSpecification<TEntity> specification)
    {
        IQueryable<TEntity> query = _dbSet.AsQueryable().WithSpecification(specification);

        var model = new PaginationModel<TEntity>()
        {
            Page = specification.PaginatedOptions.Page,
            Count = specification.PaginatedOptions.Size,
            Total = await query.CountAsync(),
            Items = await query.ToListAsync(),
        };

        return model;
    }

    public async Task<PaginationModel<TResult>> GetByPageAsync<TResult>(IPaginatedSpecification<TEntity> specification)
    {
        IQueryable<TEntity> query = _dbSet.AsQueryable().WithSpecification(specification);

        var model = new PaginationModel<TResult>()
        {
            Page = specification.PaginatedOptions.Page,
            Count = specification.PaginatedOptions.Size,
            Total = await query.CountAsync(),
            Items = await query
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync(),
        };

        return model;
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task<TResult> AddAsync<TResult>(TResult dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<TEntity>(dto);

        await _dbSet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _mapper.Map(entity, dto);
        return dto;
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ProjectTo<TResult>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
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

    public virtual async Task<TResult> UpdateAsync<TResult>(TResult dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<TEntity>(dto);

        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _mapper.Map(entity, dto);
        return dto;
    }
}