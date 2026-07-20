namespace OzoneAI.Application.Platform;

public interface ITenantLifecycleService
{
    Task<TenantDetailDto?> GetAsync(Guid companyId, CancellationToken cancellationToken = default);

    Task SuspendAsync(Guid companyId, CancellationToken cancellationToken = default);

    Task ActivateAsync(Guid companyId, CancellationToken cancellationToken = default);

    /// <summary>Used by tenant login (E3.1). Suspended/Failed/Migrating companies cannot log in.</summary>
    Task<TenantAccessCheckResult> CheckLoginAccessAsync(string companyKey, CancellationToken cancellationToken = default);

    /// <summary>Force-set the tenant Admin password (support desk; no email).</summary>
    Task ResetAdminPasswordAsync(Guid companyId, string newPassword, CancellationToken cancellationToken = default);
}

public sealed record TenantDetailDto(
    Guid Id,
    string Name,
    string CompanyKey,
    string DatabaseName,
    string Status,
    string LegacyMigrationStatus,
    string? TimeZoneId,
    Guid? PlanId,
    string? PlanName,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastUsedAt,
    int ActiveUsers30d,
    int TotalUsersCached,
    string? SchemaVersion,
    IReadOnlyList<TenantCredentialSummaryDto> Credentials);

public sealed record TenantCredentialSummaryDto(
    Guid Id,
    string Role,
    string Host,
    int Port,
    string Username,
    bool PasswordSet,
    bool IsActive,
    string? SslMode = null,
    DateTimeOffset? RotatedAt = null);

public sealed record TenantAccessCheckResult(
    bool Allowed,
    string? Reason,
    Guid? CompanyId,
    string? Status);
