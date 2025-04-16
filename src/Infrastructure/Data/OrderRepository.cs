using Microsoft.EntityFrameworkCore;

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
}