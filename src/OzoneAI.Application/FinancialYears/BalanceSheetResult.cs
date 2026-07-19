namespace OzoneAI.Application.FinancialYears;

public sealed record BalanceSheetResult(
    Guid FinancialYearId,
    string FinancialYearName,
    string CompanyName,
    string CompanyAddress,
    DateOnly AsOfDate,
    IReadOnlyList<BalanceSheetLine> Lines,
    decimal InventoryValue);
