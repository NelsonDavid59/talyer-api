using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalyerApp.Domain.Entities;
using TalyerApp.Infrastructure.Persistence;

namespace TalyerApp.Infrastructure.Persistence.Configurations;

public class BranchConfig : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.HasKey(b => new { b.Id, b.TenantId });

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(b => b.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.TenantId, 
            x.Id
        })
        .IsUnique()
        .HasDatabaseName(UniqueConstraintNames.BranchTenantIdId);

        builder.Property(b => b.Description)
            .HasMaxLength(150)
            .IsRequired();
    }
}