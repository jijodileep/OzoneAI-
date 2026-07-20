using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OzoneAI.Domain.Catalog;

namespace OzoneAI.Infrastructure.Persistence.Catalog;

internal sealed class TenantDbCredentialConfiguration : IEntityTypeConfiguration<TenantDbCredential>
{
    public void Configure(EntityTypeBuilder<TenantDbCredential> builder)
    {
        builder.ToTable("tenant_db_credentials");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Role)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(x => x.Host)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Username)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.PasswordProtected)
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(x => x.SslMode)
            .HasMaxLength(32);

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => new { x.CompanyId, x.Role })
            .IsUnique()
            .HasFilter("\"IsActive\" = TRUE");

        builder.HasOne(x => x.Company)
            .WithMany(x => x.DbCredentials)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
