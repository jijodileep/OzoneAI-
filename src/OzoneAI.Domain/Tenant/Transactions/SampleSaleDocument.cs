namespace OzoneAI.Domain.Tenant.Transactions;

/// <summary>Placeholder sale header until E4.5 full sales model. Demonstrates FY scoping.</summary>
public class SampleSaleDocument : FinancialYearScopedDocument
{
    public Guid? PartyId { get; set; }

    public decimal GrandTotal { get; set; }
}
