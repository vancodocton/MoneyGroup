using AutoMapper;
using AutoMapper.EquivalencyExpression;

using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Infrastucture.AutoMapper.Profiles;
internal class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<OrderDto, Order>(MemberList.Source)
            .ReverseMap();

        CreateMap<ConsumerDto, OrderConsumer>(MemberList.Source)
            .EqualityComparison((dto, entity) => dto.Id == entity.ConsumerId)
            .ForPath(dest => dest.ConsumerId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.OrderId, opt => opt.UseDestinationValue())
            .ReverseMap();
    }
}