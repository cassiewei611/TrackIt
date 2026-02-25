using AutoMapper;
using MediatR;
using TrackIt.Application.Common.Exceptions;
using TrackIt.Application.DTOs;
using TrackIt.Domain.Entities;
using TrackIt.Domain.Enums;
using TrackIt.Domain.Interfaces;

namespace TrackIt.Application.Commands.UpdateSubscription;

public record UpdateSubscriptionCommand(
    Guid Id,
    Guid UserId,
    string Name,
    decimal Amount,
    string CurrencyCode,
    BillingCycle BillingCycle,
    DateTime NextBillingDate,
    SubscriptionCategory Category,
    string? Notes = null
) : IRequest<SubscriptionDto>;

public class UpdateSubscriptionCommandHandler(
    ISubscriptionRepository repo,
    IUnitOfWork uow,
    IMapper mapper
) : IRequestHandler<UpdateSubscriptionCommand, SubscriptionDto>
{
    public async Task<SubscriptionDto> Handle(UpdateSubscriptionCommand request, CancellationToken ct)
    {
        var subscription = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Subscription), request.Id);

        if (subscription.UserId != request.UserId)
            throw new ForbiddenException("You don't have permission to update this subscription.");

        subscription.Update(request.Name, request.Amount, request.CurrencyCode,
            request.BillingCycle, request.NextBillingDate, request.Category, request.Notes);

        await repo.UpdateAsync(subscription, ct);
        await uow.SaveChangesAsync(ct);

        return mapper.Map<SubscriptionDto>(subscription);
    }
}
