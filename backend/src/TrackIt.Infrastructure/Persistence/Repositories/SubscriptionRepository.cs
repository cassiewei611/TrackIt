using Microsoft.EntityFrameworkCore;
using TrackIt.Domain.Entities;
using TrackIt.Domain.Interfaces;

namespace TrackIt.Infrastructure.Persistence.Repositories;

public class SubscriptionRepository(AppDbContext context) : ISubscriptionRepository
{
    public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Subscriptions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IEnumerable<Subscription>> GetByUserIdAsync(Guid userId, bool includeInactive = false, CancellationToken ct = default)
        => await context.Subscriptions
            .Where(s => s.UserId == userId && (includeInactive || s.IsActive))
            .OrderBy(s => s.NextBillingDate)
            .ToListAsync(ct);

    public async Task<IEnumerable<Subscription>> GetRenewingSoonAsync(int daysAhead = 3, CancellationToken ct = default)
    {
        var threshold = DateTime.UtcNow.AddDays(daysAhead);
        return await context.Subscriptions
            .Include(s => s.User)
            .Where(s => s.IsActive && s.NextBillingDate.Date <= threshold.Date)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Subscription subscription, CancellationToken ct = default)
        => await context.Subscriptions.AddAsync(subscription, ct);

    public Task UpdateAsync(Subscription subscription, CancellationToken ct = default)
    {
        context.Subscriptions.Update(subscription);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var subscription = await GetByIdAsync(id, ct);
        if (subscription is not null)
            context.Subscriptions.Remove(subscription);
    }
}
