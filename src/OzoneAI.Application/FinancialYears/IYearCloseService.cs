namespace OzoneAI.Application.FinancialYears;

public interface IYearCloseService
{
    /// <summary>
    /// Closes <paramref name="sourceYearId"/> and writes ledger/stock openings into the next FY
    /// (creating next year if needed). Same tenant database — no new DB.
    /// </summary>
    Task<Guid> CloseYearAsync(Guid sourceYearId, Guid? closedByUserId, CancellationToken cancellationToken = default);
}
