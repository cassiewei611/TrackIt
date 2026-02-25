using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackIt.Domain.Entities;

namespace TrackIt.Infrastructure.Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.PreferredCurrency).HasMaxLength(3).HasDefaultValue("USD");
        builder.Property(t => t.MonthlyBudgetLimit).HasColumnType("decimal(18,2)");

        builder.HasMany(t => t.Members)
            .WithOne(m => m.Team)
            .HasForeignKey(m => m.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Role)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(m => m.User)
            .WithMany(u => u.TeamMemberships)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.TeamId, m.UserId }).IsUnique();
    }
}
