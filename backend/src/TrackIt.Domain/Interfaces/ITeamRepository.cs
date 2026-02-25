using TrackIt.Domain.Entities;

namespace TrackIt.Domain.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Team?> GetByIdWithMembersAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Team>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Team team, CancellationToken ct = default);
    Task UpdateAsync(Team team, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
