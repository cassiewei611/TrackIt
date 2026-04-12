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

        var exists = await db.Users.AnyAsync(u => u.Email.Value == demoEmail);
        if (exists) return;

        logger.LogInformation("Seeding demo data...");

        var today = DateTime.UtcNow.Date;

        var alex = User.Create("Alex", "Chen", demoEmail, passwordService.HashPassword("Demo1234!"));
        alex.SetBudgetLimit(350);
        db.Users.Add(alex);

        // Subscriptions spread across past 6 months to give realistic timeline data
        var subs = new (Subscription Sub, int MonthsAgo)[]
        {
            // 6 months ago — core subs already had
            (Subscription.Create(alex.Id, "Netflix",        15.99m, "USD", BillingCycle.Monthly,  today.AddDays(4),   SubscriptionCategory.Streaming, notes: "4K plan", splitCount: 4, group: "Roommates"), 6),
            (Subscription.Create(alex.Id, "Spotify",         9.99m, "USD", BillingCycle.Monthly,  today.AddDays(12),  SubscriptionCategory.Music,     notes: "Premium individual"), 6),
            (Subscription.Create(alex.Id, "iCloud+",         2.99m, "USD", BillingCycle.Monthly,  today.AddDays(22),  SubscriptionCategory.Cloud,     notes: "200GB storage"), 6),
            (Subscription.Create(alex.Id, "NordVPN",        53.88m, "USD", BillingCycle.Yearly,   today.AddDays(210), SubscriptionCategory.Security,  notes: "2-year plan, prorated"), 6),

            // 5 months ago
            (Subscription.Create(alex.Id, "YouTube Premium",13.99m, "USD", BillingCycle.Monthly,  today.AddDays(2),   SubscriptionCategory.Streaming, notes: "Ad-free + background play"), 5),
            (Subscription.Create(alex.Id, "Amazon Prime",  139.00m, "USD", BillingCycle.Yearly,   today.AddDays(47),  SubscriptionCategory.Streaming, notes: "Shipping + Prime Video"), 5),

            // 4 months ago
            (Subscription.Create(alex.Id, "GitHub Copilot", 10.00m, "USD", BillingCycle.Monthly,  today.AddDays(18),  SubscriptionCategory.SaaS,      notes: "AI coding assistant"), 4),
            (Subscription.Create(alex.Id, "Notion Plus",     8.00m, "USD", BillingCycle.Monthly,  today.AddDays(15),  SubscriptionCategory.SaaS,      notes: "Personal workspace"), 4),
            (Subscription.Create(alex.Id, "1Password",      35.88m, "USD", BillingCycle.Yearly,   today.AddDays(91),  SubscriptionCategory.Security,  notes: "Family plan", splitCount: 2, group: "Roommates"), 4),

            // 3 months ago
            (Subscription.Create(alex.Id, "Adobe CC",       54.99m, "USD", BillingCycle.Monthly,  today.AddDays(6),   SubscriptionCategory.SaaS,      notes: "All apps plan", splitCount: 3, group: "Work"), 3),
            (Subscription.Create(alex.Id, "Vercel Pro",     20.00m, "USD", BillingCycle.Monthly,  today.AddDays(11),  SubscriptionCategory.Cloud,     notes: "Frontend deployments", splitCount: 3, group: "Work"), 3),
            (Subscription.Create(alex.Id, "Nintendo Online",19.99m, "USD", BillingCycle.Yearly,   today.AddDays(134), SubscriptionCategory.Gaming,    notes: "Individual plan"), 3),

            // 2 months ago
            (Subscription.Create(alex.Id, "ChatGPT Plus",   20.00m, "USD", BillingCycle.Monthly,  today.AddDays(9),   SubscriptionCategory.SaaS,      notes: "GPT-4o access"), 2),
            (Subscription.Create(alex.Id, "Figma",          15.00m, "USD", BillingCycle.Monthly,  today.AddDays(21),  SubscriptionCategory.SaaS,      notes: "Professional plan", splitCount: 3, group: "Work"), 2),
            (Subscription.Create(alex.Id, "Disney+",         7.99m, "USD", BillingCycle.Monthly,  today.AddDays(19),  SubscriptionCategory.Streaming, notes: "Basic plan"), 2),

            // 1 month ago
            (Subscription.Create(alex.Id, "Linear",          8.00m, "USD", BillingCycle.Monthly,  today.AddDays(30),  SubscriptionCategory.SaaS,      notes: "Issue tracker", splitCount: 3, group: "Work"), 1),
            (Subscription.Create(alex.Id, "Cloudflare Pro", 20.00m, "USD", BillingCycle.Monthly,  today.AddDays(27),  SubscriptionCategory.Cloud,     notes: "trackit.app domain"), 1),

            // This month
            (Subscription.Create(alex.Id, "Apple Music",    9.99m, "USD", BillingCycle.Monthly,  today.AddDays(25),  SubscriptionCategory.Music,     notes: "Individual plan"), 0),
            (Subscription.Create(alex.Id, "Xbox Game Pass", 14.99m, "USD", BillingCycle.Monthly,  today.AddDays(8),   SubscriptionCategory.Gaming,    notes: "PC Game Pass"), 0),
        };

        foreach (var (sub, monthsAgo) in subs)
            sub.SetCreatedAt(today.AddMonths(-monthsAgo));

        db.Subscriptions.AddRange(subs.Select(x => x.Sub));
        await db.SaveChangesAsync();

        logger.LogInformation("Demo data seeded. Email: {Email} / Password: Demo1234!", demoEmail);
    }
}
