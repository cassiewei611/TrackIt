using TrackIt.Domain.Common;
using TrackIt.Domain.Enums;
using TrackIt.Domain.Events;

namespace TrackIt.Domain.Entities;

public class Team : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public Guid OwnerId { get; private set; }
    public string PreferredCurrency { get; private set; } = "USD";
    public decimal? MonthlyBudgetLimit { get; private set; }

    public ICollection<TeamMember> Members { get; private set; } = [];
    public ICollection<Subscription> Subscriptions { get; private set; } = [];

    private Team() { }

    public static Team Create(Guid ownerId, string name, string? description = null)
    {
        var team = new Team { OwnerId = ownerId, Name = name, Description = description };
        team.Members.Add(TeamMember.Create(team.Id, ownerId, TeamRole.Owner));
        team.AddDomainEvent(new TeamCreatedEvent(team.Id, ownerId));
        return team;
    }

    public TeamMember AddMember(Guid userId, TeamRole role = TeamRole.Member)
    {
        if (Members.Any(m => m.UserId == userId))
            throw new DomainException("User is already a member of this team.");

        var member = TeamMember.Create(Id, userId, role);
        Members.Add(member);
        AddDomainEvent(new TeamMemberAddedEvent(Id, userId));
        return member;
    }

    public void RemoveMember(Guid userId)
    {
        if (OwnerId == userId)
            throw new DomainException("Cannot remove the team owner.");

        var member = Members.FirstOrDefault(m => m.UserId == userId)
            ?? throw new DomainException("Member not found.");

        Members.Remove(member);
        SetUpdated();
    }
}
