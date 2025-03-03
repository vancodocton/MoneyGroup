using AutoMapper;

using Microsoft.EntityFrameworkCore;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Infrastucture.Data;

public sealed class OrderRepository
    : EfRepository<Order>
    , IOrderRepository
{
    public OrderRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default)
    {
        return AnyAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Order?> FirstOrDefaultAsync(int id, CancellationToken cancellationToken = default)
    {
        return FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(int id, CancellationToken cancellationToken = default)
    {
        return FirstOrDefaultAsync<TResult>(o => o.Id == id, cancellationToken);
    }

    public async Task<OrderDto> UpdateAsync(OrderDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet
            .Include(o => o.Participants)
            .FirstAsync(o => o.Id == dto.Id, cancellationToken);

        _mapper.Map(dto, entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return dto;
    }
}