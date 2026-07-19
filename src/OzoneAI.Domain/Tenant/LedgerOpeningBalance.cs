namespace OzoneAI.Domain.Tenant;

public class LedgerOpeningBalance
{
    public Guid Id { get; set; }

    public Guid FinancialYearId { get; set; }

    public FinancialYear FinancialYear { get; set; } = null!;

    public Guid LedgerId { get; set; }

    /// <summary>Positive amount; side indicated by DrCr.</summary>
    public decimal OpeningAmount { get; set; }

    public DrCr DrCr { get; set; } = DrCr.Dr;

    public decimal OpeningBalancePaid { get; set; }
}
