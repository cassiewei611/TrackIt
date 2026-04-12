using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackIt.Application.Commands.CreateSubscription;
using TrackIt.Application.Commands.DeleteSubscription;
using TrackIt.Application.Commands.UpdateSubscription;
using TrackIt.Application.DTOs;
using TrackIt.Application.Queries.GetSubscriptions;
using TrackIt.Domain.Enums;

namespace TrackIt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionsController(ISender mediator) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    [ProducesResponseType<IEnumerable<SubscriptionDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool includeInactive = false,
        [FromQuery] SubscriptionCategory? category = null,
        [FromQuery] string? search = null,
        [FromQuery] string? currency = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(
            new GetSubscriptionsQuery(CurrentUserId, includeInactive, category, search, currency), ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<SubscriptionDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSubscriptionCommand command,
        CancellationToken ct)
    {
        var commandWithUser = command with { UserId = CurrentUserId };
        var result = await mediator.Send(commandWithUser, ct);
        return CreatedAtAction(nameof(GetAll), null, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<SubscriptionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSubscriptionCommand command,
        CancellationToken ct)
    {
        var commandWithIds = command with { Id = id, UserId = CurrentUserId };
        var result = await mediator.Send(commandWithIds, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteSubscriptionCommand(id, CurrentUserId), ct);
        return NoContent();
    }
}
