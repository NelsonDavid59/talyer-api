using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalyerApp.Domain.Entities;

namespace TalyerApp.Infrastructure.Persistence.Configurations;

public class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => r.Code).IsUnique();

        builder.Property(r => r.Code).HasMaxLength(150).IsRequired();

        builder.Property(r => r.RoleName).HasMaxLength(150).IsRequired();
    }
}