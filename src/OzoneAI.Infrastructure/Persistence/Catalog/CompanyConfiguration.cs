using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OzoneAI.Domain.Catalog;

namespace OzoneAI.Infrastructure.Persistence.Catalog;

internal sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.CompanyKey)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(x => x.CompanyKey)
            .IsUnique();

        builder.Property(x => x.DatabaseName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.DbHost)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.DbUsername)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.DbPasswordProtected)
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.LegacyMigrationStatus)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.TimeZoneId)
            .HasMaxLength(64);

        builder.Property(x => x.SchemaVersion)
            .HasMaxLength(64);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Plan)
            .WithMany(x => x.Companies)
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
