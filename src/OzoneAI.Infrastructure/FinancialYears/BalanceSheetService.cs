using Microsoft.EntityFrameworkCore;
using OzoneAI.Application.FinancialYears;
using OzoneAI.Domain.Tenant;
using OzoneAI.Infrastructure.Persistence.Tenant;

namespace OzoneAI.Infrastructure.FinancialYears;

public sealed class BalanceSheetService(
    TenantDbContext db,
    ILedgerBalanceCalculator ledgerBalanceCalculator,
    IStockQuantityCalculator stockQuantityCalculator) : IBalanceSheetService
{
    public async Task<BalanceSheetResult> BuildAsync(
        Guid financialYearId,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        var year = await db.FinancialYears.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == financialYearId, cancellationToken)
            ?? throw new InvalidOperationException("Financial year not found.");

        var profile = await db.CompanyProfiles.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        var openings = await db.LedgerOpeningBalances.AsNoTracking()
            .Where(x => x.FinancialYearId == financialYearId)
            .ToListAsync(cancellationToken);

        var lines = openings.Select(o =>
        {
            var openingSigned = o.DrCr == DrCr.Dr ? o.OpeningAmount : -o.OpeningAmount;
            // Movements for as-of date will come from ledger_trans when E4 lands.
            var closing = ledgerBalanceCalculator.ComputeBalance(openingSigned, movementsSigned: 0m);
            return new BalanceSheetLine(
                o.LedgerId,
                LedgerName: o.LedgerId.ToString("N")[..8],
                Section: closing >= 0 ? "Assets" : "Liabilities",
                Opening: openingSigned,
                Movements: 0m,
                Closing: closing);
        }).ToList();

        var stockOpenings = await db.StockOpeningBalances.AsNoTracking()
            .Where(x => x.FinancialYearId == financialYearId)
            .ToListAsync(cancellationToken);

        var inventory = stockOpenings.Sum(s =>
            stockQuantityCalculator.ComputeValue(s.OpeningValue, valueIn: 0, valueOut: 0));

        return new BalanceSheetResult(
            year.Id,
            year.Name,
            profile?.LegalName ?? "Company",
            profile?.Address ?? string.Empty,
            asOfDate,
            lines,
            inventory);
    }
}
