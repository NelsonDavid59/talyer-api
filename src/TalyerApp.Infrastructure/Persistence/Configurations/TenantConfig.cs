using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Tenancy;

namespace TalyerApp.Infrastructure.Persistence.Configurations;

public class TenantConfig : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Description)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(t => t.Code)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(t => t.Code).IsUnique();

        builder.HasIndex(t => t.Description).IsUnique();

        builder.Property(t => t.Type)
            .HasConversion(
                type => type.ToStorageString(),
                value => TenantTypeExtensions.FromStorageString(value))
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(t => t.Type)
            .IsUnique()
            .HasFilter($"\"Type\" = '{TenantTypeStorage.Platform}'");
    }
}
