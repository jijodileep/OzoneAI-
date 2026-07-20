namespace OzoneAI.Domain.Catalog;

/// <summary>One-time password reset token for a platform Super Admin (hashed at rest).</summary>
public class PlatformPasswordResetToken
{
    public Guid Id { get; set; }

    public Guid PlatformUserId { get; set; }

    public PlatformUser PlatformUser { get; set; } = null!;

    public string TokenHash { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UsedAt { get; set; }
}
