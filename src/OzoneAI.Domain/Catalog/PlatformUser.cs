namespace OzoneAI.Domain.Catalog;

/// <summary>
/// Catalog-scoped Super Admin account (ozone_catalog). Separate from tenant users.
/// </summary>
public class PlatformUser
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? LastLoginAt { get; set; }
}
