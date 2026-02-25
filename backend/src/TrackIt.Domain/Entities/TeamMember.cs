using TrackIt.Domain.Common;
using TrackIt.Domain.Enums;

namespace TrackIt.Domain.Entities;

public class TeamMember : BaseEntity
{
    public Guid TeamId { get; private set; }
    public Guid UserId { get; private set; }
    public TeamRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;

    public Team Team { get; private set; } = default!;
    public User User { get; private set; } = default!;

    private TeamMember() { }

    internal static TeamMember Create(Guid teamId, Guid userId, TeamRole role) =>
        new() { TeamId = teamId, UserId = userId, Role = role };

    public void UpdateRole(TeamRole newRole) { Role = newRole; SetUpdated(); }
}
