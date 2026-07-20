using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OzoneAI.Domain.Catalog;

namespace OzoneAI.Infrastructure.Persistence.Catalog;

internal sealed class PlatformPasswordResetTokenConfiguration : IEntityTypeConfiguration<PlatformPasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PlatformPasswordResetToken> builder)
    {
        builder.ToTable("platform_password_reset_tokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.TokenHash);

        builder.HasOne(x => x.PlatformUser)
            .WithMany()
            .HasForeignKey(x => x.PlatformUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
