namespace OzoneAI.Application.FinancialYears;

public sealed class StockQuantityCalculator : IStockQuantityCalculator
{
    public decimal ComputeQty(decimal openingQty, decimal qtyIn, decimal qtyOut)
        => openingQty + qtyIn - qtyOut;

    public decimal ComputeValue(decimal openingValue, decimal valueIn, decimal valueOut)
        => openingValue + valueIn - valueOut;
}
