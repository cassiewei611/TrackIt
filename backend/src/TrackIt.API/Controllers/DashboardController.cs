using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackIt.Application.DTOs;
using TrackIt.Application.Queries.GetDashboardSummary;
using TrackIt.Application.Queries.GetSpendTimeline;

namespace TrackIt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController(ISender mediator) : ControllerBase
{
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("summary")]
    [ProducesResponseType<DashboardSummaryDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(
        [FromQuery] string currency = "USD",
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetDashboardSummaryQuery(CurrentUserId, currency), ct);
        return Ok(result);
    }

    [HttpGet("timeline")]
    [ProducesResponseType<IEnumerable<SpendTimelineDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTimeline(
        [FromQuery] int months = 6,
        [FromQuery] string currency = "USD",
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetSpendTimelineQuery(CurrentUserId, months, currency), ct);
        return Ok(result);
    }
}
