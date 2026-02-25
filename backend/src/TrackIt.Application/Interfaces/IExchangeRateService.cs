namespace TrackIt.Application.Interfaces;

public interface IExchangeRateService
{
    Task<Dictionary<string, decimal>> GetRatesAsync(string baseCurrency, IEnumerable<string> targetCurrencies, CancellationToken ct = default);
    Task<decimal> GetRateAsync(string from, string to, CancellationToken ct = default);
}

public interface IEmailService
{
    Task SendRenewalReminderAsync(string email, string subscriptionName, DateTime renewalDate, decimal amount, string currency);
    Task SendBudgetAlertAsync(string email, decimal currentSpend, decimal budgetLimit, string currency);
    Task SendTeamInviteAsync(string email, string teamName, string inviterName, string inviteLink);
}

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles);
    string GenerateRefreshToken();
    Guid? ValidateRefreshToken(string token);
}

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
