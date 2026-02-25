using MediatR;
using TrackIt.Application.DTOs;
using TrackIt.Domain.Interfaces;

namespace TrackIt.Application.Queries.GetSpendTimeline;

public record GetSpendTimelineQuery(Guid UserId, int Months = 6) : IRequest<IEnumerable<SpendTimelineDto>>;

public class GetSpendTimelineQueryHandler(
    ISubscriptionRepository subscriptionRepo
) : IRequestHandler<GetSpendTimelineQuery, IEnumerable<SpendTimelineDto>>
{
    public async Task<IEnumerable<SpendTimelineDto>> Handle(GetSpendTimelineQuery request, CancellationToken ct)
    {
        var subscriptions = (await subscriptionRepo.GetByUserIdAsync(request.UserId, ct: ct)).ToList();

        var result = new List<SpendTimelineDto>();
        var now = DateTime.UtcNow;

        for (int i = request.Months - 1; i >= 0; i--)
        {
            var month = now.AddMonths(-i);
            var cutoff = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
            var total = subscriptions
                .Where(s => s.IsActive && s.CreatedAt <= cutoff)
                .Sum(s => s.MonthlyEquivalent);

            result.Add(new SpendTimelineDto(
                Month: month.ToString("MMM"),
                Year: month.Year,
                Total: Math.Round(total, 2),
                Currency: "USD"
            ));
        }

        return result;
    }
}
