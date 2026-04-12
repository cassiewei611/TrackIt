using TrackIt.Domain.Common;
using TrackIt.Domain.Enums;
using TrackIt.Domain.Events;
using TrackIt.Domain.ValueObjects;

namespace TrackIt.Domain.Entities;

public class Subscription : BaseEntity
{
    public Guid UserId { get; private set; }
    public SubscriptionName Name { get; private set; } = default!;
    public string? LogoUrl { get; private set; }
    public Money Amount { get; private set; } = default!;
    public BillingCycle BillingCycle { get; private set; }
    public DateTime NextBillingDate { get; private set; }
    public SubscriptionCategory Category { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? Notes { get; private set; }
    public int SplitCount { get; private set; } = 1;
    public string? Group { get; private set; }

    public User User { get; private set; } = default!;

    private Subscription() { }

    public static Subscription Create(
        Guid userId,
        string name,
        decimal amount,
        string currencyCode,
        BillingCycle billingCycle,
        DateTime nextBillingDate,
        SubscriptionCategory category,
        string? logoUrl = null,
        string? notes = null,
        int splitCount = 1,
        string? group = null)
    {
        var subscription = new Subscription
        {
            UserId = userId,
            Name = SubscriptionName.Create(name),
            Amount = Money.Create(amount, currencyCode),
            BillingCycle = billingCycle,
            NextBillingDate = nextBillingDate,
            Category = category,
            LogoUrl = logoUrl,
            Notes = notes,
            SplitCount = splitCount < 1 ? 1 : splitCount,
            Group = group
        };

        subscription.AddDomainEvent(new SubscriptionCreatedEvent(subscription.Id, userId, amount, currencyCode));
        return subscription;
    }

    public void Update(
        string name,
        decimal amount,
        string currencyCode,
        BillingCycle billingCycle,
        DateTime nextBillingDate,
        SubscriptionCategory category,
        string? notes = null,
        int splitCount = 1,
        string? group = null)
    {
        Name = SubscriptionName.Create(name);
        Amount = Money.Create(amount, currencyCode);
        BillingCycle = billingCycle;
        NextBillingDate = nextBillingDate;
        Category = category;
        Notes = notes;
        SplitCount = splitCount < 1 ? 1 : splitCount;
        Group = group;
        SetUpdated();
    }

    public decimal MonthlyEquivalent => BillingCycle switch
    {
        BillingCycle.Weekly    => Amount.Value * 52 / 12,
        BillingCycle.Monthly   => Amount.Value,
        BillingCycle.Quarterly => Amount.Value / 3,
        BillingCycle.Yearly    => Amount.Value / 12,
        _                      => Amount.Value
    };

    public decimal EffectiveMonthlyAmount => Math.Round(MonthlyEquivalent / SplitCount, 2);

    public bool IsRenewingSoon(int daysThreshold = 3) =>
        NextBillingDate.Date <= DateTime.UtcNow.Date.AddDays(daysThreshold);
}
