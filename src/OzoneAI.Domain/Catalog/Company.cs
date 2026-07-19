namespace OzoneAI.Domain.Catalog;

/// <summary>
/// Platform registry row in ozone_catalog. One row per tenant database.
/// Connection secrets live in <see cref="TenantDbCredential"/> — not on this row.
/// </summary>
public class Company
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>Unique login key; tenant DB name is ozone_t_{CompanyKey}.</summary>
    public string CompanyKey { get; set; } = string.Empty;

    /// <summary>Logical database name (e.g. ozone_t_demo).</summary>
    public string DatabaseName { get; set; } = string.Empty;

    public CompanyStatus Status { get; set; } = CompanyStatus.Active;

    public LegacyMigrationStatus LegacyMigrationStatus { get; set; } = LegacyMigrationStatus.NotStarted;

    public string? TimeZoneId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? LastUsedAt { get; set; }

    public int ActiveUsers30d { get; set; }

    public int TotalUsersCached { get; set; }

    public string? SchemaVersion { get; set; }

    public Guid? PlanId { get; set; }

    public SubscriptionPlan? Plan { get; set; }

    public ICollection<TenantDbCredential> DbCredentials { get; set; } = new List<TenantDbCredential>();
}
