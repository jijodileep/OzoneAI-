using OzoneAI.Infrastructure.Persistence.Tenant;

namespace OzoneAI.Infrastructure.Tenancy;

public interface ITenantDbContextFactory
{
    /// <summary>Caller owns dispose via await using.</summary>
    TenantDbContext Create(string connectionString);
}
