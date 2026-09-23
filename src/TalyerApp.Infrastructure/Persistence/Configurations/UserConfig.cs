using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalyerApp.Domain.Entities;

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
    }
}