using AutoMapper;
using AutoMapper.EquivalencyExpression;

using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Infrastructure.AutoMapper.Profiles;
internal class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<OrderDto, Order>(MemberList.Source)
            .ReverseMap();

        CreateMap<ParticipantDto, OrderParticipant>(MemberList.Source)
            .EqualityComparison((dto, entity) => dto.Id == entity.ParticipantId)
            .ForPath(dest => dest.ParticipantId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.OrderId, opt => opt.UseDestinationValue())
            .ReverseMap();

        CreateMap<Order, OrderDetailedDto>();

        CreateMap<OrderParticipant, ParticipantDetailedDto>()
            .IncludeMembers(src => src.Participant);

        CreateMap<User, ParticipantDetailedDto>();
    }
}