using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;

using Riok.Mapperly.Abstractions;

namespace MoneyGroup.Infrastructure.Mapperly;

[Mapper]
public partial class Mapper
{
    [MapperIgnoreTarget(nameof(OrderParticipant.Order))]
    [MapperIgnoreTarget(nameof(OrderParticipant.OrderId))]
    [MapperIgnoreTarget(nameof(OrderParticipant.Participant))]
    public partial OrderParticipant Map(ParticipantDto dto);

    [MapperIgnoreTarget(nameof(Order.Buyer))]
    public partial Order Map(OrderDto dto);

    [MapperIgnoreSource(nameof(OrderParticipant.Order))]
    [MapperIgnoreSource(nameof(OrderParticipant.OrderId))]
    public partial ParticipantDetailedDto Map(OrderParticipant entity);

    public partial OrderDetailedDto Map(Order entity);

    public partial IQueryable<ParticipantDetailedDto> Project(IQueryable<OrderParticipant> source);

    public partial IQueryable<OrderDetailedDto> Project(IQueryable<Order> source);
}