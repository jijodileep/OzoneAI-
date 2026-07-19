using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OzoneAI.Domain.Tenant;

namespace OzoneAI.Infrastructure.Persistence.Tenant;

internal sealed class CompanySettingsConfiguration : IEntityTypeConfiguration<CompanySettings>
{
    public void Configure(EntityTypeBuilder<CompanySettings> builder)
    {
        builder.ToTable("company_settings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TaxType).HasMaxLength(16).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(8).IsRequired();
        builder.Property(x => x.CurrencySymbol).HasMaxLength(8).IsRequired();
        builder.Property(x => x.StateCode).HasMaxLength(16);
        builder.Property(x => x.FeatureFlagsJson).HasMaxLength(4000).IsRequired();
    }
}
