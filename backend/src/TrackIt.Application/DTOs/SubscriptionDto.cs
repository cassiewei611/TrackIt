namespace TrackIt.Application.DTOs;

public record SubscriptionDto(
    Guid Id,
    string Name,
    string? LogoUrl,
    decimal Amount,
    string CurrencyCode,
    string BillingCycle,
    DateTime NextBillingDate,
    string Category,
    bool IsActive,
    string? Notes,
    decimal MonthlyEquivalent,
    DateTime CreatedAt
);

public record DashboardSummaryDto(
    int TotalActiveSubscriptions,
    decimal MonthlyTotal,
    decimal YearlyTotal,
    string Currency,
    decimal? BudgetLimit,
    double? BudgetUsedPercent,
    List<CategoryBreakdownDto> ByCategory,
    List<SubscriptionDto> RenewingSoon
);

public record CategoryBreakdownDto(string Category, decimal MonthlyAmount, int Count);

public record SpendTimelineDto(string Month, int Year, decimal Total, string Currency);

public record AuthResponseDto(string AccessToken, string RefreshToken, UserProfileDto User);

public record UserProfileDto(Guid Id, string Email, string FullName, string PreferredCurrency, decimal? MonthlyBudgetLimit);

public record TeamDto(Guid Id, string Name, string? Description, Guid OwnerId, int MemberCount, DateTime CreatedAt);

public record TeamMemberDto(Guid UserId, string Email, string FullName, string Role, DateTime JoinedAt);
