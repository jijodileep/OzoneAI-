using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OzoneAI.Application.Auth;
using OzoneAI.Application.Email;
using OzoneAI.Application.Platform;
using OzoneAI.Application.Tenancy;
using OzoneAI.Domain.Catalog;
using OzoneAI.Domain.Tenant;
using OzoneAI.Infrastructure.Persistence.Catalog;
using OzoneAI.Infrastructure.Tenancy;

namespace OzoneAI.Infrastructure.Auth;

public sealed class PasswordResetService(
    CatalogDbContext catalog,
    ITenantLifecycleService lifecycle,
    ITenantConnectionFactory connections,
    ITenantDbContextFactory tenantDbFactory,
    IPasswordHasher<TenantUser> tenantHasher,
    IPasswordHasher<PlatformUser> platformHasher,
    IEmailSender emailSender,
    IConfiguration configuration,
    ILogger<PasswordResetService> logger) : IPasswordResetService
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(1);

    public async Task RequestTenantResetAsync(
        string companyKey,
        string usernameOrEmail,
        CancellationToken cancellationToken = default)
    {
        var access = await lifecycle.CheckLoginAccessAsync(companyKey, cancellationToken);
        if (!access.Allowed || access.CompanyId is null)
        {
            return;
        }

        var lookup = usernameOrEmail.Trim();
        if (lookup.Length == 0)
        {
            return;
        }

        var company = await catalog.Companies.AsNoTracking()
            .FirstAsync(x => x.Id == access.CompanyId.Value, cancellationToken);

        string cs;
        try
        {
            cs = await connections.GetWriteConnectionStringAsync(company.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not resolve tenant connection for password reset {CompanyKey}", company.CompanyKey);
            return;
        }

        await using var tenant = tenantDbFactory.Create(cs);
        var user = await tenant.Users
            .FirstOrDefaultAsync(
                x => x.IsActive
                    && (x.Username == lookup
                        || (x.Email != null && x.Email.ToLower() == lookup.ToLower())),
                cancellationToken);

        if (user is null || string.IsNullOrWhiteSpace(user.Email))
        {
            return;
        }

        var rawToken = CreateRawToken();
        var tokenHash = HashToken(rawToken);

        var existing = await tenant.PasswordResetTokens
            .Where(x => x.UserId == user.Id && x.UsedAt == null)
            .ToListAsync(cancellationToken);
        tenant.PasswordResetTokens.RemoveRange(existing);

        tenant.PasswordResetTokens.Add(new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = tokenHash,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.Add(TokenLifetime)
        });
        await tenant.SaveChangesAsync(cancellationToken);

        var baseUrl = configuration["App:PublicWebBaseUrl"] ?? "http://localhost:5173";
        var link =
            $"{baseUrl.TrimEnd('/')}/reset-password?companyKey={Uri.EscapeDataString(company.CompanyKey)}&token={Uri.EscapeDataString(rawToken)}";

        await emailSender.SendAsync(
            new EmailMessage(
                user.Email,
                "Reset your OzoneAI password",
                $"Hi {user.DisplayName},\n\nUse this link to reset your password (expires in 1 hour):\n{link}\n\nIf you did not request this, ignore this email.\n",
                user.DisplayName),
            cancellationToken);
    }

    public async Task ResetTenantPasswordAsync(
        string companyKey,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token is required.");
        }

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
        {
            throw new ArgumentException("New password must be at least 8 characters.");
        }

        var access = await lifecycle.CheckLoginAccessAsync(companyKey, cancellationToken);
        if (!access.Allowed || access.CompanyId is null)
        {
            throw new InvalidOperationException("Invalid or expired reset token.");
        }

        var cs = await connections.GetWriteConnectionStringAsync(access.CompanyId.Value, cancellationToken);
        await using var tenant = tenantDbFactory.Create(cs);
        var tokenHash = HashToken(token.Trim());
        var row = await tenant.PasswordResetTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash && x.UsedAt == null,
                cancellationToken);

        if (row is null || row.ExpiresAt < DateTimeOffset.UtcNow || !row.User.IsActive)
        {
            throw new InvalidOperationException("Invalid or expired reset token.");
        }

        row.User.PasswordHash = tenantHasher.HashPassword(row.User, newPassword);
        row.UsedAt = DateTimeOffset.UtcNow;
        await tenant.SaveChangesAsync(cancellationToken);
    }

    public async Task RequestPlatformResetAsync(
        string usernameOrEmail,
        CancellationToken cancellationToken = default)
    {
        var lookup = usernameOrEmail.Trim();
        if (lookup.Length == 0)
        {
            return;
        }

        var user = await catalog.PlatformUsers
            .FirstOrDefaultAsync(
                x => x.IsActive
                    && (x.Username == lookup
                        || (x.Email != null && x.Email.ToLower() == lookup.ToLower())),
                cancellationToken);

        if (user is null || string.IsNullOrWhiteSpace(user.Email))
        {
            return;
        }

        var rawToken = CreateRawToken();
        var tokenHash = HashToken(rawToken);

        var existing = await catalog.PlatformPasswordResetTokens
            .Where(x => x.PlatformUserId == user.Id && x.UsedAt == null)
            .ToListAsync(cancellationToken);
        catalog.PlatformPasswordResetTokens.RemoveRange(existing);

        catalog.PlatformPasswordResetTokens.Add(new PlatformPasswordResetToken
        {
            Id = Guid.NewGuid(),
            PlatformUserId = user.Id,
            TokenHash = tokenHash,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.Add(TokenLifetime)
        });
        await catalog.SaveChangesAsync(cancellationToken);

        var baseUrl = configuration["App:PublicWebBaseUrl"] ?? "http://localhost:5173";
        var link =
            $"{baseUrl.TrimEnd('/')}/super-admin/reset-password?token={Uri.EscapeDataString(rawToken)}";

        await emailSender.SendAsync(
            new EmailMessage(
                user.Email,
                "Reset your OzoneAI Super Admin password",
                $"Hi {user.DisplayName},\n\nUse this link to reset your password (expires in 1 hour):\n{link}\n\nIf you did not request this, ignore this email.\n",
                user.DisplayName),
            cancellationToken);
    }

    public async Task ResetPlatformPasswordAsync(
        string token,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token is required.");
        }

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
        {
            throw new ArgumentException("New password must be at least 8 characters.");
        }

        var tokenHash = HashToken(token.Trim());
        var row = await catalog.PlatformPasswordResetTokens
            .Include(x => x.PlatformUser)
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash && x.UsedAt == null,
                cancellationToken);

        if (row is null || row.ExpiresAt < DateTimeOffset.UtcNow || !row.PlatformUser.IsActive)
        {
            throw new InvalidOperationException("Invalid or expired reset token.");
        }

        row.PlatformUser.PasswordHash = platformHasher.HashPassword(row.PlatformUser, newPassword);
        row.UsedAt = DateTimeOffset.UtcNow;
        await catalog.SaveChangesAsync(cancellationToken);
    }

    private static string CreateRawToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static string HashToken(string rawToken)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
