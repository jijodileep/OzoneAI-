namespace OzoneAI.Domain.Tenant;

public class StockOpeningBalance
{
    public Guid Id { get; set; }

    public Guid FinancialYearId { get; set; }

    public FinancialYear FinancialYear { get; set; } = null!;

    public Guid ItemId { get; set; }

    public Guid GodownId { get; set; }

    public string? BatchNo { get; set; }

    public decimal OpeningQty { get; set; }

    public decimal OpeningRate { get; set; }

    public decimal OpeningValue { get; set; }
}
