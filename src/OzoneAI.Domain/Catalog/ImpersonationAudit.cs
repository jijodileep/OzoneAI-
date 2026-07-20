namespace OzoneAI.Domain.Catalog;

/// <summary>Catalog audit row when a Super Admin opens a tenant session.</summary>
public class ImpersonationAudit
{
    public Guid Id { get; set; }

    public Guid PlatformUserId { get; set; }

    public Guid CompanyId { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public string? IpAddress { get; set; }

    public PlatformUser? PlatformUser { get; set; }

    public Company? Company { get; set; }
}
