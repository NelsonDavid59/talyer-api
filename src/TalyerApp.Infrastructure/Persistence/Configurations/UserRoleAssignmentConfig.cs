using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalyerApp.Domain.Entities;

namespace TalyerApp.Infrastructure.Persistence.Configurations;

public class UserRoleAssignmentConfig : IEntityTypeConfiguration<UserRoleAssignment>
{
    public void Configure(EntityTypeBuilder<UserRoleAssignment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Branch)
            .WithMany()
            .HasForeignKey(x => new
            {
                x.TenantId,
                x.BranchId
            })
            .HasPrincipalKey(x => new
            {
                x.TenantId,
                x.Id
            })
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.RoleId, x.TenantId })
            .IsUnique()
            .HasDatabaseName(UniqueConstraintNames.UserRoleAssignmentTenantWide)
            .HasFilter("\"BranchId\" IS NULL");

        builder.HasIndex(x => new { x.UserId, x.RoleId, x.TenantId, x.BranchId })
            .IsUnique()
            .HasDatabaseName(UniqueConstraintNames.UserRoleAssignmentPerBranch)
            .HasFilter("\"BranchId\" IS NOT NULL");
    }
}
