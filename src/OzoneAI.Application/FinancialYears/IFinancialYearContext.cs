namespace OzoneAI.Application.FinancialYears;

public interface IFinancialYearContext
{
    Guid? FinancialYearId { get; }

    bool IsReadOnly { get; }

    void Set(Guid financialYearId, bool isReadOnly);
}
