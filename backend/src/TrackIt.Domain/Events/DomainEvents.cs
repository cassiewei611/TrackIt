using TrackIt.Domain.Common;

namespace TrackIt.Domain.Events;

public record UserCreatedEvent(Guid UserId, string Email) : IDomainEvent;
public record SubscriptionCreatedEvent(Guid SubscriptionId, Guid UserId, decimal Amount, string Currency) : IDomainEvent;
