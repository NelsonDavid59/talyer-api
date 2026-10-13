using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Membership;

namespace TalyerApp.Infrastructure.Persistence.Configurations;

public class MembershipRequestConfig : IEntityTypeConfiguration<MembershipRequest>
{
    public void Configure(EntityTypeBuilder<MembershipRequest> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CompanyName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(100).IsRequired();
        builder.Property(x => x.FirstName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.EmailVerificationTokenHash).HasMaxLength(64).IsRequired();
        builder.Property(x => x.RejectedReason).HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasConversion(
                status => status.ToString(),
                value => Enum.Parse<MembershipRequestStatus>(value))
            .HasMaxLength(40)
            .IsRequired();

        builder.HasIndex(x => x.EmailVerificationTokenHash);

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasFilter(
                "\"Status\" IN ('PendingEmailVerification', 'PendingReview')")
            .HasDatabaseName("IX_MembershipRequests_Email_Active");
    }
}
