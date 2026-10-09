using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalyerApp.Domain.Entities;
using TalyerApp.Infrastructure.Persistence;

namespace TalyerApp.Infrastructure.Persistence.Configurations;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.HasMany(u => u.ExternalIdentities)
            .WithOne(ei => ei.User)
            .HasForeignKey(ei => ei.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(u => u.Email).HasMaxLength(100).IsRequired();
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName(UniqueConstraintNames.UserEmail);
        builder.Property(u => u.Username).HasMaxLength(150).IsRequired();
        builder.Property(u => u.FirstName).HasMaxLength(150).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(150).IsRequired();
    }
}