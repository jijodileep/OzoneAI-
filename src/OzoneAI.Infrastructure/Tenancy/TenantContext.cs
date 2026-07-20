using OzoneAI.Application.Tenancy;
using OzoneAI.Domain.Catalog;

namespace OzoneAI.Infrastructure.Tenancy;

public sealed class TenantContext : ITenantContext
{
    public Guid? CompanyId { get; private set; }

    public string? CompanyKey { get; private set; }

    public string? ConnectionString { get; private set; }

    public TenantDbCredentialRole? ConnectionRole { get; private set; }

    public Guid? UserId { get; private set; }

    public string? Username { get; private set; }

    public string? Role { get; private set; }

    public Guid? ImpersonatedBy { get; private set; }

    public void Set(
        Guid companyId,
        string companyKey,
        string connectionString,
        TenantDbCredentialRole connectionRole,
        Guid? userId = null,
        string? username = null,
        string? role = null,
        Guid? impersonatedBy = null)
    {
        CompanyId = companyId;
        CompanyKey = companyKey;
        ConnectionString = connectionString;
        ConnectionRole = connectionRole;
        UserId = userId;
        Username = username;
        Role = role;
        ImpersonatedBy = impersonatedBy;
    }
}
