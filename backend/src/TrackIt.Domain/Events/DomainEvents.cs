using TrackIt.Domain.Common;

namespace TrackIt.Domain.Events;

public record UserCreatedEvent(Guid UserId, string Email) : IDomainEvent;
public record SubscriptionCreatedEvent(Guid SubscriptionId, Guid UserId, decimal Amount, string Currency) : IDomainEvent;
public record SubscriptionDeactivatedEvent(Guid SubscriptionId, Guid UserId) : IDomainEvent;
public record TeamCreatedEvent(Guid TeamId, Guid OwnerId) : IDomainEvent;
public record TeamMemberAddedEvent(Guid TeamId, Guid UserId) : IDomainEvent;
public record BudgetExceededEvent(Guid UserId, decimal CurrentSpend, decimal BudgetLimit) : IDomainEvent;
