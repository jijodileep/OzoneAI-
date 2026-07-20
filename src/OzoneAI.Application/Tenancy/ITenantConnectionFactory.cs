using OzoneAI.Domain.Catalog;

namespace OzoneAI.Application.Tenancy;

public interface ITenantConnectionFactory
{
    /// <summary>
    /// Builds Npgsql connection string from catalog credential rows + company DatabaseName.
    /// Read role falls back to Write when no active Read credential exists.
    /// </summary>
    Task<string> GetConnectionStringAsync(
        Guid companyId,
        TenantDbCredentialRole role = TenantDbCredentialRole.Write,
        CancellationToken cancellationToken = default);

    Task<string> GetWriteConnectionStringAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<string> GetReadConnectionStringAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    /// <summary>Drops cached Write/Read strings for a company (after credential rotate).</summary>
    void InvalidateCache(Guid companyId);
}

public sealed record TenantConnectionInfo(
    Guid CompanyId,
    string CompanyKey,
    string DatabaseName,
    TenantDbCredentialRole Role,
    string ConnectionString,
    bool UsedReadFallback);
