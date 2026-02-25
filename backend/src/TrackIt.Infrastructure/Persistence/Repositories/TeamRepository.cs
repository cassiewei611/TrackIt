using Microsoft.EntityFrameworkCore;
using TrackIt.Domain.Entities;
using TrackIt.Domain.Interfaces;

namespace TrackIt.Infrastructure.Persistence.Repositories;

public class TeamRepository(AppDbContext context) : ITeamRepository
{
    public async Task<Team?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Teams.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<Team?> GetByIdWithMembersAsync(Guid id, CancellationToken ct = default)
        => await context.Teams
            .Include(t => t.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IEnumerable<Team>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await context.Teams
            .Include(t => t.Members)
            .Where(t => t.Members.Any(m => m.UserId == userId))
            .ToListAsync(ct);

    public async Task AddAsync(Team team, CancellationToken ct = default)
        => await context.Teams.AddAsync(team, ct);

    public Task UpdateAsync(Team team, CancellationToken ct = default)
    {
        context.Teams.Update(team);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var team = await GetByIdAsync(id, ct);
        if (team is not null)
            context.Teams.Remove(team);
    }
}
