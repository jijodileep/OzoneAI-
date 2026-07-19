namespace OzoneAI.Application.Platform;

public interface IPlatformAuthService
{
    Task<PlatformLoginResult?> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
}

public sealed record PlatformLoginResult(
    string AccessToken,
    int ExpiresInSeconds,
    PlatformUserDto User);

public sealed record PlatformUserDto(Guid Id, string Username, string DisplayName);
