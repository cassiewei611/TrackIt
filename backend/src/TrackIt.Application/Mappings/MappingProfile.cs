using AutoMapper;
using TrackIt.Application.DTOs;
using TrackIt.Domain.Entities;

namespace TrackIt.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Subscription, SubscriptionDto>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name.Value))
            .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount.Value))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.Amount.CurrencyCode))
            .ForMember(d => d.BillingCycle, o => o.MapFrom(s => s.BillingCycle.ToString()))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.ToString()));

        CreateMap<User, UserProfileDto>()
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email.Value))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName));

        CreateMap<Team, TeamDto>()
            .ForMember(d => d.MemberCount, o => o.MapFrom(s => s.Members.Count));

        CreateMap<TeamMember, TeamMemberDto>()
            .ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email.Value))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.User.FullName))
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));
    }
}
