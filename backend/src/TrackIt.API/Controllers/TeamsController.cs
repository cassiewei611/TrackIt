using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackIt.Application.Commands.Teams;
using TrackIt.Application.DTOs;
using TrackIt.Application.Queries.GetTeams;

namespace TrackIt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeamsController(ISender mediator) : ControllerBase
{
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    [ProducesResponseType<IEnumerable<TeamDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTeams(CancellationToken ct)
    {
        var result = await mediator.Send(new GetTeamsQuery(CurrentUserId), ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<TeamDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTeamCommand command, CancellationToken ct)
    {
        var commandWithOwner = command with { OwnerId = CurrentUserId };
        var result = await mediator.Send(commandWithOwner, ct);
        return CreatedAtAction(nameof(GetMyTeams), new { id = result.Id }, result);
    }
}
