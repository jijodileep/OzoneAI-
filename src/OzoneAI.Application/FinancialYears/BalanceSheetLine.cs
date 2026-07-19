namespace OzoneAI.Application.FinancialYears;

public sealed record BalanceSheetLine(
    Guid LedgerId,
    string LedgerName,
    string Section,
    decimal Opening,
    decimal Movements,
    decimal Closing);
