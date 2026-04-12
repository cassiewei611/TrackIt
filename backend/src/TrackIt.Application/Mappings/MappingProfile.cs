using AutoMapper;
using TrackIt.Application.DTOs;
using TrackIt.Domain.Entities;

namespace TrackIt.Application.Mappings;

// AutoMapper profile kept for DI registration; all actual mapping is done inline in handlers.
public class MappingProfile : Profile
{
    public MappingProfile() { }
}
