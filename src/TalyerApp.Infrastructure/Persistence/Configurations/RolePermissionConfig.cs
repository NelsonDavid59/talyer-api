using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalyerApp.Domain.Entities;

namespace TalyerApp.Infrastructure.Persistence.Configurations;

public class RolePermissionConfig : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        // Composed PK
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
                .IsUnique()
                .HasDatabaseName(UniqueConstraintNames.RolePermissionRoleIdPermissionId);

        // Many to many relationship
        builder.HasOne<Role>(rp => rp.Role)
                .WithMany()
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Permission>(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}