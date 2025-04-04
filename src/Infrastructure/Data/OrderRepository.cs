﻿using Microsoft.EntityFrameworkCore;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Infrastructure.Mapperly;

namespace MoneyGroup.Infrastructure.Data;

public sealed class OrderRepository
    : EfRepository<Order>
    , IOrderRepository
{
    public OrderRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public async Task<OrderDto> AddAsync(OrderDto dto, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Order>(dto);

        await _dbSet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Missing feature: tracking issue https://github.com/riok/mapperly/issues/884
        dto.Id = entity.Id;
        return dto;
    }

    public async Task<OrderDto> UpdateAsync(OrderDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet
            .Include(o => o.Participants)
            .ThenInclude(op => op.Participant)
            .FirstAsync(o => o.Id == dto.Id, cancellationToken);

        // Missing feature: tracking issue https://github.com/riok/mapperly/issues/884
        ((Mapper)_mapper).Map(dto, entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return dto;
    }
}