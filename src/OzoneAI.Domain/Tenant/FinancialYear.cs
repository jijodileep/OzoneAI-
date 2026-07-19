namespace OzoneAI.Domain.Tenant;

public class FinancialYear
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public FinancialYearStatus Status { get; set; } = FinancialYearStatus.Open;

    public bool IsDefault { get; set; }

    public DateTimeOffset? ClosedAt { get; set; }

    public Guid? ClosedByUserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<LedgerOpeningBalance> LedgerOpeningBalances { get; set; } = new List<LedgerOpeningBalance>();

    public ICollection<StockOpeningBalance> StockOpeningBalances { get; set; } = new List<StockOpeningBalance>();
}
