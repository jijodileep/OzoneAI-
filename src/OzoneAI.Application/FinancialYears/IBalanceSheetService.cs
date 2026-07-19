namespace OzoneAI.Application.FinancialYears;

public interface IBalanceSheetService
{
    Task<BalanceSheetResult> BuildAsync(Guid financialYearId, DateOnly asOfDate, CancellationToken cancellationToken = default);
}
