namespace OzoneAI.Application.FinancialYears;

public sealed record FinancialYearSwitchResult(
    Guid FinancialYearId,
    string Name,
    bool IsReadOnly,
    string AccessToken);

public interface IFinancialYearSwitchService
{
    Task<FinancialYearSwitchResult> SwitchAsync(Guid financialYearId, CancellationToken cancellationToken = default);
}
