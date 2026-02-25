using AutoMapper;
using MediatR;
using TrackIt.Application.Common.Exceptions;
using TrackIt.Application.DTOs;
using TrackIt.Domain.Entities;
using TrackIt.Domain.Interfaces;

namespace TrackIt.Application.Queries.GetSubscriptionById;

public record GetSubscriptionByIdQuery(Guid Id, Guid UserId) : IRequest<SubscriptionDto>;

public class GetSubscriptionByIdQueryHandler(
    ISubscriptionRepository repo,
    IMapper mapper
) : IRequestHandler<GetSubscriptionByIdQuery, SubscriptionDto>
{
    public async Task<SubscriptionDto> Handle(GetSubscriptionByIdQuery request, CancellationToken ct)
    {
        var subscription = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Subscription), request.Id);

        if (subscription.UserId != request.UserId)
            throw new ForbiddenException("You don't have permission to view this subscription.");

        return mapper.Map<SubscriptionDto>(subscription);
    }
}
