using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OzoneAI.Domain.Tenant;

namespace OzoneAI.Infrastructure.Persistence.Tenant;

internal sealed class StockOpeningBalanceConfiguration : IEntityTypeConfiguration<StockOpeningBalance>
{
    public void Configure(EntityTypeBuilder<StockOpeningBalance> builder)
    {
        builder.ToTable("stock_opening_balances");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.BatchNo).HasMaxLength(64);
        builder.Property(x => x.OpeningQty).HasPrecision(18, 4);
        builder.Property(x => x.OpeningRate).HasPrecision(18, 4);
        builder.Property(x => x.OpeningValue).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.FinancialYearId, x.ItemId, x.GodownId, x.BatchNo }).IsUnique();
        builder.HasOne(x => x.FinancialYear)
            .WithMany(x => x.StockOpeningBalances)
            .HasForeignKey(x => x.FinancialYearId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
