namespace OzoneAI.Application.FinancialYears;

public sealed class LedgerBalanceCalculator : ILedgerBalanceCalculator
{
    public decimal ComputeBalance(decimal openingSigned, decimal movementsSigned)
        => openingSigned + movementsSigned;
}
