namespace OzoneAI.Application.Platform;

public interface ITenantProvisioningService
{
    Task<CreateTenantResult> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TenantSummaryDto>> ListAsync(CancellationToken cancellationToken = default);
}

public static class TenantDbModes
{
    public const string Provisioned = "Provisioned";
    public const string External = "External";
}

public sealed record CreateTenantRequest(
    string Name,
    string CompanyKey,
    string AdminUsername,
    string AdminPassword,
    string? AdminDisplayName = null,
    string? AdminEmail = null,
    Guid? PlanId = null,
    string? TimeZoneId = null,
    string? LegalName = null,
    string? Address = null,
    string? Phone = null,
    string? Email = null,
    string? TaxType = null,
    string? CurrencyCode = null,
    /// <summary><see cref="TenantDbModes.Provisioned"/> (default) or <see cref="TenantDbModes.External"/>.</summary>
    string? DbMode = null,
    string? DbHost = null,
    int? DbPort = null,
    string? DatabaseName = null,
    string? DbUsername = null,
    string? DbPassword = null,
    string? SslMode = null);

public sealed record CreateTenantResult(
    Guid CompanyId,
    string CompanyKey,
    string DatabaseName,
    string Status,
    string AdminUsername);

public sealed record TenantSummaryDto(
    Guid Id,
    string Name,
    string CompanyKey,
    string DatabaseName,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastUsedAt,
    int ActiveUsers30d,
    int TotalUsersCached);
