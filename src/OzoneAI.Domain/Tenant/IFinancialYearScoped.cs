namespace OzoneAI.Domain.Tenant;

/// <summary>Marker for transactional entities that must carry FinancialYearId.</summary>
public interface IFinancialYearScoped
{
    Guid FinancialYearId { get; set; }
}
