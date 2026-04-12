using MediatR;
using Microsoft.Extensions.Logging;
using TrackIt.Application.Common.Exceptions;
using TrackIt.Application.DTOs;
using TrackIt.Application.Interfaces;
using TrackIt.Domain.Entities;
using TrackIt.Domain.Enums;
using TrackIt.Domain.Interfaces;

namespace TrackIt.Application.Commands.CreateSubscription;

public class CreateSubscriptionCommandHandler(
    ISubscriptionRepository subscriptionRepository,
    IUserRepository userRepository,
    IExchangeRateService exchangeRateService,
    IUnitOfWork unitOfWork,
    ILogger<CreateSubscriptionCommandHandler> logger
) : IRequestHandler<CreateSubscriptionCommand, SubscriptionDto>
{
    public async Task<SubscriptionDto> Handle(CreateSubscriptionCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        await CheckBudgetThresholdAsync(user, request, ct);

        var subscription = Subscription.Create(
            userId: request.UserId,
            name: request.Name,
            amount: request.Amount,
            currencyCode: request.CurrencyCode,
            billingCycle: request.BillingCycle,
            nextBillingDate: request.NextBillingDate,
            category: request.Category,
            logoUrl: request.LogoUrl,
            notes: request.Notes,
            splitCount: request.SplitCount,
            group: request.Group
        );

        await subscriptionRepository.AddAsync(subscription, ct);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Subscription {Name} created for user {UserId}", request.Name, request.UserId);

        return new SubscriptionDto(
            subscription.Id, subscription.Name.Value, subscription.LogoUrl,
            subscription.Amount.Value, subscription.Amount.CurrencyCode,
            subscription.BillingCycle.ToString(), subscription.NextBillingDate,
            subscription.Category.ToString(), subscription.IsActive,
            subscription.Notes, subscription.MonthlyEquivalent, subscription.CreatedAt,
            subscription.SplitCount, subscription.Group, subscription.EffectiveMonthlyAmount
        );
    }

    private async Task CheckBudgetThresholdAsync(User user, CreateSubscriptionCommand request, CancellationToken ct)
    {
        if (user.MonthlyBudgetLimit is null) return;

        var existing = await subscriptionRepository.GetByUserIdAsync(user.Id, ct: ct);
        var currentMonthlyTotal = existing.Sum(s => s.EffectiveMonthlyAmount);

        var newMonthlyAmount = request.BillingCycle switch
        {
            BillingCycle.Yearly => request.Amount / 12,
            BillingCycle.Weekly => request.Amount * 52 / 12,
            BillingCycle.Quarterly => request.Amount / 3,
            _ => request.Amount
        } / (request.SplitCount < 1 ? 1 : request.SplitCount);

        if (currentMonthlyTotal + newMonthlyAmount > user.MonthlyBudgetLimit)
            logger.LogWarning("User {UserId} budget threshold will be exceeded", user.Id);
    }
}
