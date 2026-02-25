using FluentValidation;

namespace TrackIt.Application.Commands.CreateSubscription;

public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    private static readonly string[] ValidCurrencies = ["USD", "EUR", "GBP", "DKK", "SEK", "NOK", "JPY", "CHF", "CAD", "AUD"];

    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Subscription name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.")
            .LessThanOrEqualTo(100_000).WithMessage("Amount seems unreasonably large.");

        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .Length(3).WithMessage("Currency must be a 3-letter ISO code.")
            .Must(c => ValidCurrencies.Contains(c.ToUpper())).WithMessage("Unsupported currency code.");

        RuleFor(x => x.NextBillingDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date.AddDays(-1)).WithMessage("Next billing date cannot be in the past.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
