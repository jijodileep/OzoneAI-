using OzoneAI.Application.FinancialYears;

namespace OzoneAI.Infrastructure.FinancialYears;

public sealed class FinancialYearContext : IFinancialYearContext
{
    public Guid? FinancialYearId { get; private set; }

    public bool IsReadOnly { get; private set; }

    public void Set(Guid financialYearId, bool isReadOnly)
    {
        FinancialYearId = financialYearId;
        IsReadOnly = isReadOnly;
    }
}
