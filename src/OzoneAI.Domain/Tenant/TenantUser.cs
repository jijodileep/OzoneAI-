namespace OzoneAI.Domain.Tenant;

/// <summary>
/// Tenant-scoped login account (inside ozone_t_*). Separate from catalog PlatformUser.
/// </summary>
public class TenantUser
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Role { get; set; } = TenantRoles.Admin;

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? LastLoginAt { get; set; }
}

public static class TenantRoles
{
    public const string Admin = "Admin";
}
