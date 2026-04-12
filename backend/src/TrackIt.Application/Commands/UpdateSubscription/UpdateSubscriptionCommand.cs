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
    string? Notes = null,
    int SplitCount = 1,
    string? Group = null
) : IRequest<SubscriptionDto>;

public class UpdateSubscriptionCommandHandler(
    ISubscriptionRepository repo,
    IUnitOfWork uow
) : IRequestHandler<UpdateSubscriptionCommand, SubscriptionDto>
{
    public async Task<SubscriptionDto> Handle(UpdateSubscriptionCommand request, CancellationToken ct)
    {
        var subscription = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Subscription), request.Id);

        if (subscription.UserId != request.UserId)
            throw new ForbiddenException("You don't have permission to update this subscription.");

        subscription.Update(request.Name, request.Amount, request.CurrencyCode,
            request.BillingCycle, request.NextBillingDate, request.Category,
            request.Notes, request.SplitCount, request.Group);

        await repo.UpdateAsync(subscription, ct);
        await uow.SaveChangesAsync(ct);

        return new SubscriptionDto(
            subscription.Id, subscription.Name.Value, subscription.LogoUrl,
            subscription.Amount.Value, subscription.Amount.CurrencyCode,
            subscription.BillingCycle.ToString(), subscription.NextBillingDate,
            subscription.Category.ToString(), subscription.IsActive,
            subscription.Notes, subscription.MonthlyEquivalent, subscription.CreatedAt,
            subscription.SplitCount, subscription.Group, subscription.EffectiveMonthlyAmount
        );
    }
}
