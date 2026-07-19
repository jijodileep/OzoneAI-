using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OzoneAI.Domain.Tenant;

namespace OzoneAI.Infrastructure.Persistence.Tenant;

internal sealed class LedgerOpeningBalanceConfiguration : IEntityTypeConfiguration<LedgerOpeningBalance>
{
    public void Configure(EntityTypeBuilder<LedgerOpeningBalance> builder)
    {
        builder.ToTable("ledger_opening_balances");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OpeningAmount).HasPrecision(18, 2);
        builder.Property(x => x.OpeningBalancePaid).HasPrecision(18, 2);
        builder.Property(x => x.DrCr).HasConversion<string>().HasMaxLength(2).IsRequired();
        builder.HasIndex(x => new { x.FinancialYearId, x.LedgerId }).IsUnique();
        builder.HasOne(x => x.FinancialYear)
            .WithMany(x => x.LedgerOpeningBalances)
            .HasForeignKey(x => x.FinancialYearId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
