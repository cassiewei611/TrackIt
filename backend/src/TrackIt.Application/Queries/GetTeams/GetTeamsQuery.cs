using AutoMapper;
using MediatR;
using TrackIt.Application.DTOs;
using TrackIt.Domain.Interfaces;

namespace TrackIt.Application.Queries.GetTeams;

public record GetTeamsQuery(Guid UserId) : IRequest<IEnumerable<TeamDto>>;

public class GetTeamsQueryHandler(
    ITeamRepository teamRepository,
    IMapper mapper
) : IRequestHandler<GetTeamsQuery, IEnumerable<TeamDto>>
{
    public async Task<IEnumerable<TeamDto>> Handle(GetTeamsQuery request, CancellationToken ct)
    {
        var teams = await teamRepository.GetByUserIdAsync(request.UserId, ct);
        return mapper.Map<IEnumerable<TeamDto>>(teams);
    }
}
