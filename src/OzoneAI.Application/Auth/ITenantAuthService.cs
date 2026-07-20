namespace OzoneAI.Application.Auth;

public interface ITenantAuthService
{
    Task<TenantLoginResult?> LoginAsync(
        string companyKey,
        string username,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>Issues a tenant JWT for an existing active user (used by impersonation).</summary>
    Task<TenantLoginResult> IssueTokenForUserAsync(
        Guid companyId,
        Guid userId,
        TimeSpan lifetime,
        Guid? impersonatedByPlatformUserId = null,
        CancellationToken cancellationToken = default);
}

public sealed record TenantLoginResult(
    string AccessToken,
    int ExpiresInSeconds,
    TenantAuthUserDto User,
    TenantAuthCompanyDto Company,
    Guid? ImpersonatedBy);

public sealed record TenantAuthUserDto(Guid Id, string Username, string DisplayName, string Role);

public sealed record TenantAuthCompanyDto(Guid Id, string CompanyKey, string Name);
