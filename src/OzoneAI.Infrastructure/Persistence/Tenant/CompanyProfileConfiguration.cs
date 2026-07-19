using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OzoneAI.Domain.Tenant;

namespace OzoneAI.Infrastructure.Persistence.Tenant;

internal sealed class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
{
    public void Configure(EntityTypeBuilder<CompanyProfile> builder)
    {
        builder.ToTable("company_profile");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LegalName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.TradeName).HasMaxLength(200);
        builder.Property(x => x.Address).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.AddressLocal).HasMaxLength(4000);
        builder.Property(x => x.Phone).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(256);
        builder.Property(x => x.TaxNumber).HasMaxLength(64);
        builder.Property(x => x.Fssai).HasMaxLength(64);
        builder.Property(x => x.LogoObjectKey).HasMaxLength(512);
        builder.Property(x => x.BankName).HasMaxLength(200);
        builder.Property(x => x.BankAccountNo).HasMaxLength(64);
        builder.Property(x => x.BankIfsc).HasMaxLength(32);
    }
}
