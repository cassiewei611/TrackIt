using TrackIt.Domain.Common;
using TrackIt.Domain.Events;
using TrackIt.Domain.ValueObjects;

namespace TrackIt.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string PreferredCurrency { get; private set; } = "USD";
    public decimal? MonthlyBudgetLimit { get; private set; }

    public ICollection<Subscription> Subscriptions { get; private set; } = [];
    public ICollection<TeamMember> TeamMemberships { get; private set; } = [];

    private User() { }

    public static User Create(string firstName, string lastName, string email, string passwordHash)
    {
        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = Email.Create(email),
            PasswordHash = passwordHash
        };
        user.AddDomainEvent(new UserCreatedEvent(user.Id, email));
        return user;
    }

    public void UpdateProfile(string firstName, string lastName, string preferredCurrency)
    {
        FirstName = firstName;
        LastName = lastName;
        PreferredCurrency = preferredCurrency;
        SetUpdated();
    }

    public void SetBudgetLimit(decimal? monthlyLimit)
    {
        MonthlyBudgetLimit = monthlyLimit;
        SetUpdated();
    }

    public string FullName => $"{FirstName} {LastName}";
}
