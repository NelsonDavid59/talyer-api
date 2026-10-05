using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalyerApp.Domain.Entities;

namespace TalyerApp.Infrastructure.Persistence.Configurations;

public class PermissionConfig : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.Code).IsUnique();

        builder.Property(p => p.Code).HasMaxLength(150).IsRequired();
    }
}