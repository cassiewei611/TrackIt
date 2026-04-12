using Microsoft.EntityFrameworkCore;
using TrackIt.Domain.Entities;
using TrackIt.Domain.Interfaces;
using TrackIt.Infrastructure.Persistence.Models;

namespace TrackIt.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<ExchangeRateCache> ExchangeRates => Set<ExchangeRateCache>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await base.SaveChangesAsync(ct);
    }
}
