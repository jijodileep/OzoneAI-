namespace OzoneAI.Application.FinancialYears;

public interface IStockQuantityCalculator
{
    /// <summary>OpeningQty + In - Out for the financial year.</summary>
    decimal ComputeQty(decimal openingQty, decimal qtyIn, decimal qtyOut);

    /// <summary>Default valuation: weighted value from opening value + purchase-like ins.</summary>
    decimal ComputeValue(decimal openingValue, decimal valueIn, decimal valueOut);
}
