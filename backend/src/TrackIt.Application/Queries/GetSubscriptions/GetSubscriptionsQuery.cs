using MediatR;
using TrackIt.Application.DTOs;
using TrackIt.Application.Interfaces;
using TrackIt.Domain.Enums;
using TrackIt.Domain.Interfaces;

namespace TrackIt.Application.Queries.GetSubscriptions;

public record GetSubscriptionsQuery(
    Guid UserId,
    bool IncludeInactive = false,
    SubscriptionCategory? Category = null,
    string? SearchTerm = null,
    string? TargetCurrency = null
) : IRequest<IEnumerable<SubscriptionDto>>;

public class GetSubscriptionsQueryHandler(
    ISubscriptionRepository repo,
    IExchangeRateService exchangeRateService
) : IRequestHandler<GetSubscriptionsQuery, IEnumerable<SubscriptionDto>>
{
    public async Task<IEnumerable<SubscriptionDto>> Handle(GetSubscriptionsQuery request, CancellationToken ct)
    {
        var subscriptions = await repo.GetByUserIdAsync(request.UserId, request.IncludeInactive, ct);

        if (request.Category.HasValue)
            subscriptions = subscriptions.Where(s => s.Category == request.Category.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            subscriptions = subscriptions.Where(s =>
                s.Name.Value.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));

        var subList = subscriptions.ToList();

        Dictionary<string, decimal>? rates = null;
        if (!string.IsNullOrWhiteSpace(request.TargetCurrency))
        {
            var currencies = subList.Select(s => s.Amount.CurrencyCode).Distinct().ToList();
            rates = await exchangeRateService.GetRatesAsync(request.TargetCurrency, currencies, ct);
        }

        return subList.Select(s =>
        {
            decimal? converted = null;
            if (rates != null && request.TargetCurrency != null &&
                !string.Equals(s.Amount.CurrencyCode, request.TargetCurrency, StringComparison.OrdinalIgnoreCase))
            {
                var rate = rates.GetValueOrDefault(s.Amount.CurrencyCode, 1m);
                converted = Math.Round(s.EffectiveMonthlyAmount * rate, 2);
            }

            return new SubscriptionDto(
                s.Id, s.Name.Value, s.LogoUrl, s.Amount.Value, s.Amount.CurrencyCode,
                s.BillingCycle.ToString(), s.NextBillingDate, s.Category.ToString(),
                s.IsActive, s.Notes, s.MonthlyEquivalent, s.CreatedAt,
                s.SplitCount, s.Group, s.EffectiveMonthlyAmount,
                converted, request.TargetCurrency
            );
        });
    }
}
