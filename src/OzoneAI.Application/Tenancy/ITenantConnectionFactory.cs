using OzoneAI.Domain.Catalog;

namespace OzoneAI.Application.Tenancy;

public interface ITenantConnectionFactory
{
    /// <summary>Builds Npgsql connection string from catalog credential rows + company DatabaseName.</summary>
    Task<string> GetConnectionStringAsync(
        Guid companyId,
        TenantDbCredentialRole role = TenantDbCredentialRole.Write,
        CancellationToken cancellationToken = default);
}
