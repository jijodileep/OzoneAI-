using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneAI.Application.Auth;
using OzoneAI.Application.Platform;
using OzoneAI.Domain.Catalog;
using OzoneAI.Infrastructure.Auth;
using OzoneAI.Infrastructure.Persistence.Catalog;

namespace OzoneAI.Infrastructure.Platform;

public sealed class PlatformAuthService(
    CatalogDbContext catalog,
    IJwtTokenService jwt,
    IPasswordHasher<PlatformUser> passwordHasher) : IPlatformAuthService
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);

    public async Task<PlatformLoginResult?> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalized = username.Trim();
        if (normalized.Length == 0 || string.IsNullOrEmpty(password))
        {
            return null;
        }

        var user = await catalog.PlatformUsers
            .FirstOrDefaultAsync(x => x.Username == normalized, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var verify = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verify == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (verify == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, password);
        }

        user.LastLoginAt = DateTimeOffset.UtcNow;
        await catalog.SaveChangesAsync(cancellationToken);

        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, JwtTokenService.SuperAdminRole),
            new(JwtTokenService.AuthScopeClaim, JwtTokenService.PlatformScope)
        };

        var token = jwt.IssueToken(claims, TokenLifetime);
        return new PlatformLoginResult(
            token,
            (int)TokenLifetime.TotalSeconds,
            new PlatformUserDto(user.Id, user.Username, user.DisplayName));
    }
}
