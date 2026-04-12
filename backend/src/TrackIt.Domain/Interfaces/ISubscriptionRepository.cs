using TrackIt.Domain.Entities;

namespace TrackIt.Domain.Interfaces;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Subscription>> GetByUserIdAsync(Guid userId, bool includeInactive = false, CancellationToken ct = default);
    Task<IEnumerable<Subscription>> GetRenewingSoonAsync(int daysAhead = 3, CancellationToken ct = default);
    Task AddAsync(Subscription subscription, CancellationToken ct = default);
    Task UpdateAsync(Subscription subscription, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
