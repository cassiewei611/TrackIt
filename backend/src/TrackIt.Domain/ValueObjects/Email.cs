using System.Text.RegularExpressions;

namespace TrackIt.Domain.ValueObjects;

public sealed class Email
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; private set; } = default!;

    private Email() { }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        if (!EmailRegex.IsMatch(email))
            throw new ArgumentException($"'{email}' is not a valid email address.", nameof(email));

        return new Email { Value = email.ToLowerInvariant() };
    }

    public override string ToString() => Value;
}
