namespace OzoneAI.Application.Platform;

public interface IImpersonationService
{
    Task<ImpersonationResult> ImpersonateAsync(
        Guid companyId,
        Guid platformUserId,
        string reason,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ImpersonationAuditDto>> ListAuditsAsync(
        Guid companyId,
        int take = 20,
        CancellationToken cancellationToken = default);
}

public sealed record ImpersonationResult(
    string AccessToken,
    int ExpiresInSeconds,
    Guid AuditId,
    Guid ImpersonatedBy,
    TenantAuthCompanyBrief Company,
    TenantAuthUserBrief User);

public sealed record TenantAuthCompanyBrief(Guid Id, string CompanyKey, string Name);

public sealed record TenantAuthUserBrief(Guid Id, string Username, string DisplayName, string Role);

public sealed record ImpersonationAuditDto(
    Guid Id,
    Guid PlatformUserId,
    string? PlatformUsername,
    string Reason,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt,
    string? IpAddress);
