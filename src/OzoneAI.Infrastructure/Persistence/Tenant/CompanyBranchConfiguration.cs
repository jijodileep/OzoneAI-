using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OzoneAI.Domain.Tenant;

namespace OzoneAI.Infrastructure.Persistence.Tenant;

internal sealed class CompanyBranchConfiguration : IEntityTypeConfiguration<CompanyBranch>
{
    public void Configure(EntityTypeBuilder<CompanyBranch> builder)
    {
        builder.ToTable("company_branches");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(32);
        builder.Property(x => x.Email).HasMaxLength(256);
        builder.Property(x => x.TaxNumber).HasMaxLength(64);
    }
}
