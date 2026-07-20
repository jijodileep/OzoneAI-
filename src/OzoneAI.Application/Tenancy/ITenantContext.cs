using OzoneAI.Domain.Catalog;

namespace OzoneAI.Application.Tenancy;

/// <summary>Per-request tenant identity and connection (from JWT after auth middleware).</summary>
public interface ITenantContext
{
    Guid? CompanyId { get; }

    string? CompanyKey { get; }

    /// <summary>Resolved Npgsql connection string for the current company, if set.</summary>
    string? ConnectionString { get; }

    /// <summary>Which credential role was used to build <see cref="ConnectionString"/>.</summary>
    TenantDbCredentialRole? ConnectionRole { get; }

    Guid? UserId { get; }

    string? Username { get; }

    string? Role { get; }

    Guid? ImpersonatedBy { get; }

    void Set(
        Guid companyId,
        string companyKey,
        string connectionString,
        TenantDbCredentialRole connectionRole,
        Guid? userId = null,
        string? username = null,
        string? role = null,
        Guid? impersonatedBy = null);
}
