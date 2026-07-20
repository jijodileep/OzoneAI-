namespace OzoneAI.Domain.Catalog;

/// <summary>
/// Tenant database connection credentials in ozone_catalog (separate from companies registry).
/// </summary>
public class TenantDbCredential
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    public Company Company { get; set; } = null!;

    public TenantDbCredentialRole Role { get; set; } = TenantDbCredentialRole.Write;

    public string Host { get; set; } = "postgres-catalog";

    public int Port { get; set; } = 5432;

    public string Username { get; set; } = string.Empty;

    /// <summary>Encrypted at rest in later stories; plain for local scaffold only.</summary>
    public string PasswordProtected { get; set; } = string.Empty;

    /// <summary>
    /// Optional Npgsql SSL Mode (e.g. Prefer, Require). Null/empty = omit from connection string.
    /// </summary>
    public string? SslMode { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? RotatedAt { get; set; }
}
