using Microsoft.EntityFrameworkCore;
using TrackIt.Domain.Entities;
using TrackIt.Domain.Enums;
using TrackIt.Application.Interfaces;
using TrackIt.Infrastructure.Persistence;

namespace TrackIt.API.Extensions;

public static class DatabaseExtensions
{
    public static async Task MigrateAndSeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

        try
        {
            await db.Database.MigrateAsync();
            logger.LogInformation("Database migration completed successfully.");
            await SeedDemoDataAsync(db, passwordService, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }

    private static async Task SeedDemoDataAsync(AppDbContext db, IPasswordService passwordService, ILogger logger)
    {
        const string demoEmail = "demo@trackit.app";

        // Skip if demo user already exists
        var exists = await db.Users.AnyAsync(u => u.Email.Value == demoEmail);
        if (exists) return;

        logger.LogInformation("Seeding demo user and subscriptions...");

        var user = User.Create("Alex", "Chen", demoEmail, passwordService.HashPassword("Demo1234!"));
        user.SetBudgetLimit(300);
        db.Users.Add(user);

        var today = DateTime.UtcNow.Date;

        var subscriptions = new[]
        {
            Subscription.Create(user.Id, "Netflix",          15.99m, "USD", BillingCycle.Monthly,  today.AddDays(4),   SubscriptionCategory.Streaming, notes: "4K plan"),
            Subscription.Create(user.Id, "Spotify",           9.99m, "USD", BillingCycle.Monthly,  today.AddDays(12),  SubscriptionCategory.Music,     notes: "Premium individual"),
            Subscription.Create(user.Id, "GitHub Copilot",   10.00m, "USD", BillingCycle.Monthly,  today.AddDays(18),  SubscriptionCategory.SaaS,      notes: "AI coding assistant"),
            Subscription.Create(user.Id, "Adobe CC",         54.99m, "USD", BillingCycle.Monthly,  today.AddDays(6),   SubscriptionCategory.SaaS,      notes: "All apps plan"),
            Subscription.Create(user.Id, "iCloud+",           2.99m, "USD", BillingCycle.Monthly,  today.AddDays(22),  SubscriptionCategory.Cloud,     notes: "200GB storage"),
            Subscription.Create(user.Id, "YouTube Premium",  13.99m, "USD", BillingCycle.Monthly,  today.AddDays(2),   SubscriptionCategory.Streaming, notes: "Ad-free + downloads"),
            Subscription.Create(user.Id, "Amazon Prime",    139.00m, "USD", BillingCycle.Yearly,   today.AddDays(47),  SubscriptionCategory.Streaming, notes: "Shipping + Prime Video"),
            Subscription.Create(user.Id, "Notion Plus",       8.00m, "USD", BillingCycle.Monthly,  today.AddDays(15),  SubscriptionCategory.SaaS,      notes: "Personal workspace"),
            Subscription.Create(user.Id, "1Password",        35.88m, "USD", BillingCycle.Yearly,   today.AddDays(91),  SubscriptionCategory.Security,  notes: "Family plan ÷ 2"),
            Subscription.Create(user.Id, "ChatGPT Plus",     20.00m, "USD", BillingCycle.Monthly,  today.AddDays(9),   SubscriptionCategory.SaaS,      notes: "GPT-4o access"),
            Subscription.Create(user.Id, "Nintendo Online",  19.99m, "USD", BillingCycle.Yearly,   today.AddDays(134), SubscriptionCategory.Gaming,    notes: "Individual plan"),
            Subscription.Create(user.Id, "Cloudflare Pro",   20.00m, "USD", BillingCycle.Monthly,  today.AddDays(27),  SubscriptionCategory.Cloud,     notes: "trackit.app domain"),
        };

        db.Subscriptions.AddRange(subscriptions);
        await db.SaveChangesAsync();

        logger.LogInformation("Demo user seeded. Email: {Email} / Password: Demo1234!", demoEmail);
    }
}
