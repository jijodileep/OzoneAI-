using OzoneAI.Domain.Tenant;

namespace OzoneAI.Application.FinancialYears;

public interface IFinancialYearGuard
{
    /// <summary>Throws if document date is outside FY or FY is closed for posting.</summary>
    void EnsureCanPost(FinancialYear year, DateOnly documentDate);

    bool IsDateInYear(FinancialYear year, DateOnly documentDate);
}
