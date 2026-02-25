namespace TrackIt.Domain.ValueObjects;

public sealed class Money
{
    public decimal Value { get; private set; }
    public string CurrencyCode { get; private set; } = default!;

    private Money() { }

    public static Money Create(decimal value, string currencyCode)
    {
        if (value < 0)
            throw new ArgumentException("Money value cannot be negative.", nameof(value));
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Length != 3)
            throw new ArgumentException("Currency code must be a 3-letter ISO code.", nameof(currencyCode));

        return new Money { Value = value, CurrencyCode = currencyCode.ToUpperInvariant() };
    }

    public override string ToString() => $"{Value:F2} {CurrencyCode}";
}
