using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalyerApp.Domain.Entities;

namespace TalyerApp.Infrastructure.Persistence.Configurations;

public class TenantConfig : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Description)
                .HasMaxLength(150)
                .IsRequired();

        builder.HasIndex(t => t.Description).IsUnique();
    }
}