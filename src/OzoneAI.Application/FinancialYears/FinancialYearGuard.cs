using OzoneAI.Domain.Tenant;

namespace OzoneAI.Application.FinancialYears;

public sealed class FinancialYearGuard : IFinancialYearGuard
{
    public bool IsDateInYear(FinancialYear year, DateOnly documentDate)
        => documentDate >= year.StartDate && documentDate <= year.EndDate;

    public void EnsureCanPost(FinancialYear year, DateOnly documentDate)
    {
        if (year.Status != FinancialYearStatus.Open)
        {
            throw new InvalidOperationException(
                $"Cannot post into financial year '{year.Name}' with status {year.Status}.");
        }

        if (!IsDateInYear(year, documentDate))
        {
            throw new InvalidOperationException(
                $"Document date {documentDate:yyyy-MM-dd} is outside financial year '{year.Name}' ({year.StartDate:yyyy-MM-dd}–{year.EndDate:yyyy-MM-dd}).");
        }
    }
}
