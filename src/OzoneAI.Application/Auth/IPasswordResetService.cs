namespace OzoneAI.Application.Auth;

public interface IPasswordResetService
{
    /// <summary>Always succeeds from the caller's perspective (anti-enumeration).</summary>
    Task RequestTenantResetAsync(string companyKey, string usernameOrEmail, CancellationToken cancellationToken = default);

    Task ResetTenantPasswordAsync(
        string companyKey,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task RequestPlatformResetAsync(string usernameOrEmail, CancellationToken cancellationToken = default);

    Task ResetPlatformPasswordAsync(
        string token,
        string newPassword,
        CancellationToken cancellationToken = default);
}
