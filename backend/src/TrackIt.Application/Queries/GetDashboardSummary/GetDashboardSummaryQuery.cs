using MediatR;
using TrackIt.Application.Common.Exceptions;
using TrackIt.Application.DTOs;
using TrackIt.Application.Interfaces;
using TrackIt.Domain.Entities;
using TrackIt.Domain.Interfaces;

namespace TrackIt.Application.Queries.GetDashboardSummary;

public record GetDashboardSummaryQuery(Guid UserId, string TargetCurrency = "USD") : IRequest<DashboardSummaryDto>;

public class GetDashboardSummaryQueryHandler(
    ISubscriptionRepository subscriptionRepo,
    IUserRepository userRepo,
    IExchangeRateService exchangeRateService
) : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken ct)
    {
        var user = await userRepo.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        var subscriptions = (await subscriptionRepo.GetByUserIdAsync(request.UserId, ct: ct)).ToList();

        var currencies = subscriptions.Select(s => s.Amount.CurrencyCode).Distinct().ToList();
        var rates = await exchangeRateService.GetRatesAsync(request.TargetCurrency, currencies, ct);

        var monthlyTotal = subscriptions.Sum(s =>
        {
            var rate = rates.GetValueOrDefault(s.Amount.CurrencyCode, 1m);
            return s.EffectiveMonthlyAmount * rate;
        });

        var yearlyTotal = monthlyTotal * 12;

        var byCategory = subscriptions
            .GroupBy(s => s.Category)
            .Select(g => new CategoryBreakdownDto(
                Category: g.Key.ToString(),
                MonthlyAmount: Math.Round(g.Sum(s => s.EffectiveMonthlyAmount * rates.GetValueOrDefault(s.Amount.CurrencyCode, 1m)), 2),
                Count: g.Count()
            ))
            .OrderByDescending(x => x.MonthlyAmount)
            .ToList();

        var renewingSoon = subscriptions
            .Where(s => s.IsRenewingSoon(7))
            .OrderBy(s => s.NextBillingDate)
            .Take(5)
            .Select(s => new SubscriptionDto(
                s.Id, s.Name.Value, s.LogoUrl, s.Amount.Value, s.Amount.CurrencyCode,
                s.BillingCycle.ToString(), s.NextBillingDate, s.Category.ToString(),
                s.IsActive, s.Notes, s.MonthlyEquivalent, s.CreatedAt,
                s.SplitCount, s.Group, s.EffectiveMonthlyAmount
            ))
            .ToList();

        return new DashboardSummaryDto(
            TotalActiveSubscriptions: subscriptions.Count(s => s.IsActive),
            MonthlyTotal: Math.Round(monthlyTotal, 2),
            YearlyTotal: Math.Round(yearlyTotal, 2),
            Currency: request.TargetCurrency,
            BudgetLimit: user.MonthlyBudgetLimit,
            BudgetUsedPercent: user.MonthlyBudgetLimit.HasValue
                ? Math.Round((double)(monthlyTotal / user.MonthlyBudgetLimit.Value) * 100, 1)
                : null,
            ByCategory: byCategory,
            RenewingSoon: renewingSoon
        );
    }
}
