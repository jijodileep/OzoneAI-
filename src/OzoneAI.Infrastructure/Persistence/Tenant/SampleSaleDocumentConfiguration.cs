using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OzoneAI.Domain.Tenant.Transactions;

namespace OzoneAI.Infrastructure.Persistence.Tenant;

internal sealed class SampleSaleDocumentConfiguration : IEntityTypeConfiguration<SampleSaleDocument>
{
    public void Configure(EntityTypeBuilder<SampleSaleDocument> builder)
    {
        builder.ToTable("sample_sales");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DocumentNo).HasMaxLength(64).IsRequired();
        builder.Property(x => x.GrandTotal).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.FinancialYearId, x.DocumentNo }).IsUnique();
        builder.HasIndex(x => new { x.FinancialYearId, x.DocumentDate });
    }
}
