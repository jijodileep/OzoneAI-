namespace OzoneAI.Application.FinancialYears;

public interface ILedgerBalanceCalculator
{
    /// <summary>Opening(FY) + sum of signed movements in FY.</summary>
    decimal ComputeBalance(decimal openingSigned, decimal movementsSigned);
}
