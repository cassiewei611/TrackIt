using MediatR;
using TrackIt.Application.DTOs;
using TrackIt.Domain.Enums;

namespace TrackIt.Application.Commands.CreateSubscription;

public record CreateSubscriptionCommand(
    Guid UserId,
    string Name,
    decimal Amount,
    string CurrencyCode,
    BillingCycle BillingCycle,
    DateTime NextBillingDate,
    SubscriptionCategory Category,
    string? LogoUrl = null,
    string? Notes = null,
    Guid? TeamId = null
) : IRequest<SubscriptionDto>;
