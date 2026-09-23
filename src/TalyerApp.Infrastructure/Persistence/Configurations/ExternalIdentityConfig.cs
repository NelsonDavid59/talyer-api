using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalyerApp.Domain.Entities;

namespace TalyerApp.Infrastructure.Persistence.Configurations;

public class ExternalIdentityConfig : IEntityTypeConfiguration<ExternalIdentity>
{
    public void Configure(EntityTypeBuilder<ExternalIdentity> builder)
    {
        builder.HasKey(ei => new {ei.UserId, ei.Provider, ei.ProviderUserId});

        // Unique constraint for Provider and ProviderUserId keys
        builder.HasIndex(ei => new {ei.Provider, ei.ProviderUserId})
                .IsUnique();
    }
}