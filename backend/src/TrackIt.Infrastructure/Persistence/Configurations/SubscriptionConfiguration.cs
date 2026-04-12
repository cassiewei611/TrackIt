using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackIt.Domain.Entities;

namespace TrackIt.Infrastructure.Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(s => s.Id);

        builder.OwnsOne(s => s.Name, name =>
        {
            name.Property(n => n.Value)
                .HasColumnName("Name")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.OwnsOne(s => s.Amount, money =>
        {
            money.Property(m => m.Value)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            money.Property(m => m.CurrencyCode)
                .HasColumnName("CurrencyCode")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(s => s.BillingCycle)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.Category)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.Notes).HasMaxLength(500);
        builder.Property(s => s.LogoUrl).HasMaxLength(500);
        builder.Property(s => s.SplitCount).HasDefaultValue(1);
        builder.Property(s => s.Group).HasMaxLength(100);

        builder.HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.NextBillingDate);
        builder.HasIndex(s => new { s.UserId, s.IsActive });
    }
}
