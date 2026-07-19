namespace OzoneAI.Domain.Tenant.Transactions;

/// <summary>
/// Base for Wave-1 transactional documents. Concrete sales/purchase/voucher entities inherit this.
/// </summary>
public abstract class FinancialYearScopedDocument : IFinancialYearScoped
{
    public Guid Id { get; set; }

    public Guid FinancialYearId { get; set; }

    public DateOnly DocumentDate { get; set; }

    public string DocumentNo { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
}
