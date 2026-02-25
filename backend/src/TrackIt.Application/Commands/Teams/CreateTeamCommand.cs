using AutoMapper;
using FluentValidation;
using MediatR;
using TrackIt.Application.Common.Exceptions;
using TrackIt.Application.DTOs;
using TrackIt.Domain.Entities;
using TrackIt.Domain.Interfaces;

namespace TrackIt.Application.Commands.Teams;

public record CreateTeamCommand(Guid OwnerId, string Name, string? Description = null) : IRequest<TeamDto>;

public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.OwnerId).NotEmpty();
    }
}

public class CreateTeamCommandHandler(
    ITeamRepository teamRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper
) : IRequestHandler<CreateTeamCommand, TeamDto>
{
    public async Task<TeamDto> Handle(CreateTeamCommand request, CancellationToken ct)
    {
        var owner = await userRepository.GetByIdAsync(request.OwnerId, ct)
            ?? throw new NotFoundException(nameof(User), request.OwnerId);

        var team = Team.Create(request.OwnerId, request.Name, request.Description);
        await teamRepository.AddAsync(team, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return mapper.Map<TeamDto>(team);
    }
}
