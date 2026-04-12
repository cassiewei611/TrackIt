using MediatR;
using TrackIt.Application.Common.Exceptions;
using TrackIt.Domain.Entities;
using TrackIt.Domain.Interfaces;

namespace TrackIt.Application.Commands.DeleteSubscription;

public record DeleteSubscriptionCommand(Guid Id, Guid UserId) : IRequest;

public class DeleteSubscriptionCommandHandler(
    ISubscriptionRepository repo,
    IUnitOfWork uow
) : IRequestHandler<DeleteSubscriptionCommand>
{
    public async Task Handle(DeleteSubscriptionCommand request, CancellationToken ct)
    {
        var subscription = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Subscription), request.Id);

        if (subscription.UserId != request.UserId)
            throw new ForbiddenException("You don't have permission to delete this subscription.");

        await repo.DeleteAsync(request.Id, ct);
        await uow.SaveChangesAsync(ct);
    }
}
