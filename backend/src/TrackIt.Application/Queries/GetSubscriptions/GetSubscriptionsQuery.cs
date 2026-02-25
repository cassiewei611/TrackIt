using AutoMapper;
using MediatR;
using TrackIt.Application.DTOs;
using TrackIt.Domain.Enums;
using TrackIt.Domain.Interfaces;

namespace TrackIt.Application.Queries.GetSubscriptions;

public record GetSubscriptionsQuery(
    Guid UserId,
    bool IncludeInactive = false,
    SubscriptionCategory? Category = null,
    string? SearchTerm = null
) : IRequest<IEnumerable<SubscriptionDto>>;

public class GetSubscriptionsQueryHandler(
    ISubscriptionRepository repo,
    IMapper mapper
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

        return mapper.Map<IEnumerable<SubscriptionDto>>(subscriptions);
    }
}
