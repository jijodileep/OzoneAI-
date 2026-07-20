using Microsoft.EntityFrameworkCore;
using OzoneAI.Application.Auth;
using OzoneAI.Application.Platform;
using OzoneAI.Application.Tenancy;
using OzoneAI.Domain.Catalog;
using OzoneAI.Domain.Tenant;
using OzoneAI.Infrastructure.FinancialYears;
using OzoneAI.Infrastructure.Persistence.Catalog;
using OzoneAI.Infrastructure.Persistence.Tenant;

namespace OzoneAI.Infrastructure.Platform;

public sealed class ImpersonationService(
    CatalogDbContext catalog,
    ITenantLifecycleService lifecycle,
    ITenantConnectionFactory connectionFactory,
    ITenantAuthService tenantAuth) : IImpersonationService
{
    private static readonly TimeSpan ImpersonationLifetime = TimeSpan.FromHours(1);

    public async Task<ImpersonationResult> ImpersonateAsync(
        Guid companyId,
        Guid platformUserId,
        string reason,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var trimmedReason = reason.Trim();
        if (trimmedReason.Length < 3)
        {
            throw new ArgumentException("Reason must be at least 3 characters.");
        }

        if (trimmedReason.Length > 500)
        {
            throw new ArgumentException("Reason must be at most 500 characters.");
        }

        var company = await catalog.Companies.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken)
            ?? throw new KeyNotFoundException("Company not found.");

        var access = await lifecycle.CheckLoginAccessAsync(company.CompanyKey, cancellationToken);
        if (!access.Allowed)
        {
            throw new InvalidOperationException(access.Reason ?? "Company cannot be impersonated.");
        }

        var platformUser = await catalog.PlatformUsers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == platformUserId && x.IsActive, cancellationToken)
            ?? throw new InvalidOperationException("Platform user not found.");

        var cs = await connectionFactory.GetConnectionStringAsync(
            companyId,
            TenantDbCredentialRole.Write,
            cancellationToken);

        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(cs)
            .Options;

        await using var tenantDb = new TenantDbContext(options, new FinancialYearContext());
        var admin = await tenantDb.Users.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Role == TenantRoles.Admin ? 0 : 1)
            .ThenBy(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("No active tenant user to impersonate.");

        var expiresAt = DateTimeOffset.UtcNow.Add(ImpersonationLifetime);
        var audit = new ImpersonationAudit
        {
            Id = Guid.NewGuid(),
            PlatformUserId = platformUser.Id,
            CompanyId = company.Id,
            Reason = trimmedReason,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = expiresAt,
            IpAddress = ipAddress
        };
        catalog.ImpersonationAudits.Add(audit);
        await catalog.SaveChangesAsync(cancellationToken);

        var tokenResult = await tenantAuth.IssueTokenForUserAsync(
            company.Id,
            admin.Id,
            ImpersonationLifetime,
            platformUser.Id,
            cancellationToken);

        return new ImpersonationResult(
            tokenResult.AccessToken,
            tokenResult.ExpiresInSeconds,
            audit.Id,
            platformUser.Id,
            new TenantAuthCompanyBrief(
                tokenResult.Company.Id,
                tokenResult.Company.CompanyKey,
                tokenResult.Company.Name),
            new TenantAuthUserBrief(
                tokenResult.User.Id,
                tokenResult.User.Username,
                tokenResult.User.DisplayName,
                tokenResult.User.Role));
    }

    public async Task<IReadOnlyList<ImpersonationAuditDto>> ListAuditsAsync(
        Guid companyId,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 100);
        return await catalog.ImpersonationAudits.AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(take)
            .Select(x => new ImpersonationAuditDto(
                x.Id,
                x.PlatformUserId,
                x.PlatformUser != null ? x.PlatformUser.Username : null,
                x.Reason,
                x.CreatedAt,
                x.ExpiresAt,
                x.IpAddress))
            .ToListAsync(cancellationToken);
    }
}
