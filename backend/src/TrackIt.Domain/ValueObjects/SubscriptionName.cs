namespace TrackIt.Domain.ValueObjects;

public sealed class SubscriptionName
{
    public string Value { get; private set; } = default!;

    private SubscriptionName() { }

    public static SubscriptionName Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Subscription name cannot be empty.", nameof(name));
        if (name.Length > 100)
            throw new ArgumentException("Subscription name cannot exceed 100 characters.", nameof(name));

        return new SubscriptionName { Value = name.Trim() };
    }

    public override string ToString() => Value;
}
