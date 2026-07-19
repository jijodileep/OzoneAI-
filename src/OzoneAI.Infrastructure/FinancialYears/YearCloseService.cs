using Microsoft.EntityFrameworkCore;
using OzoneAI.Application.FinancialYears;
using OzoneAI.Domain.Tenant;
using OzoneAI.Infrastructure.Persistence.Tenant;

namespace OzoneAI.Infrastructure.FinancialYears;

public sealed class YearCloseService(
    TenantDbContext db,
    ILedgerBalanceCalculator ledgerBalanceCalculator,
    IStockQuantityCalculator stockQuantityCalculator) : IYearCloseService
{
    public async Task<Guid> CloseYearAsync(Guid sourceYearId, Guid? closedByUserId, CancellationToken cancellationToken = default)
    {
        var source = await db.FinancialYears
            .Include(x => x.LedgerOpeningBalances)
            .Include(x => x.StockOpeningBalances)
            .FirstOrDefaultAsync(x => x.Id == sourceYearId, cancellationToken)
            ?? throw new InvalidOperationException("Source financial year not found.");

        if (source.Status == FinancialYearStatus.Closed)
        {
            throw new InvalidOperationException("Financial year is already closed.");
        }

        source.Status = FinancialYearStatus.Closing;

        var nextStart = source.EndDate.AddDays(1);
        var nextEnd = nextStart.AddYears(1).AddDays(-1);
        var nextName = $"{nextStart.Year}-{nextEnd.Year.ToString()[^2..]}";

        var next = await db.FinancialYears.FirstOrDefaultAsync(
            x => x.StartDate == nextStart && x.EndDate == nextEnd,
            cancellationToken);

        if (next is null)
        {
            next = new FinancialYear
            {
                Id = Guid.NewGuid(),
                Name = nextName,
                StartDate = nextStart,
                EndDate = nextEnd,
                Status = FinancialYearStatus.Open,
                IsDefault = true,
                CreatedAt = DateTimeOffset.UtcNow
            };
            db.FinancialYears.Add(next);
        }
        else
        {
            next.Status = FinancialYearStatus.Open;
            next.IsDefault = true;
        }

        // Closing ledger balance ≈ opening for next FY (movements hook when ledger_trans exists).
        foreach (var lob in source.LedgerOpeningBalances)
        {
            var signedOpening = lob.DrCr == DrCr.Dr ? lob.OpeningAmount : -lob.OpeningAmount;
            var closing = ledgerBalanceCalculator.ComputeBalance(signedOpening, movementsSigned: 0m);
            var drCr = closing >= 0 ? DrCr.Dr : DrCr.Cr;
            db.LedgerOpeningBalances.Add(new LedgerOpeningBalance
            {
                Id = Guid.NewGuid(),
                FinancialYearId = next.Id,
                LedgerId = lob.LedgerId,
                OpeningAmount = Math.Abs(closing),
                DrCr = drCr,
                OpeningBalancePaid = 0
            });
        }

        foreach (var sob in source.StockOpeningBalances)
        {
            var qty = stockQuantityCalculator.ComputeQty(sob.OpeningQty, qtyIn: 0, qtyOut: 0);
            var value = stockQuantityCalculator.ComputeValue(sob.OpeningValue, valueIn: 0, valueOut: 0);
            var rate = qty == 0 ? 0 : value / qty;
            db.StockOpeningBalances.Add(new StockOpeningBalance
            {
                Id = Guid.NewGuid(),
                FinancialYearId = next.Id,
                ItemId = sob.ItemId,
                GodownId = sob.GodownId,
                BatchNo = sob.BatchNo,
                OpeningQty = qty,
                OpeningRate = rate,
                OpeningValue = value
            });
        }

        source.Status = FinancialYearStatus.Closed;
        source.ClosedAt = DateTimeOffset.UtcNow;
        source.ClosedByUserId = closedByUserId;
        source.IsDefault = false;

        await db.SaveChangesAsync(cancellationToken);
        return next.Id;
    }
}
