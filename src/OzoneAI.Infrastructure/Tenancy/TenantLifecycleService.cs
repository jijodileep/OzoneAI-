using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneAI.Application.Platform;
using OzoneAI.Application.Tenancy;
using OzoneAI.Domain.Catalog;
using OzoneAI.Domain.Tenant;
using OzoneAI.Infrastructure.Persistence.Catalog;

namespace OzoneAI.Infrastructure.Tenancy;

public sealed class TenantLifecycleService(
    CatalogDbContext catalog,
    ITenantConnectionFactory connections,
    ITenantDbContextFactory tenantDbFactory,
    IPasswordHasher<TenantUser> passwordHasher) : ITenantLifecycleService
{
    public async Task<TenantDetailDto?> GetAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var company = await catalog.Companies.AsNoTracking()
            .Include(x => x.Plan)
            .Include(x => x.DbCredentials)
            .FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken);

        if (company is null)
        {
            return null;
        }

        return MapDetail(company);
    }

    public async Task SuspendAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var company = await catalog.Companies.FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken)
            ?? throw new InvalidOperationException("Company not found.");

        if (company.Status == CompanyStatus.Suspended)
        {
            return;
        }

        if (company.Status is CompanyStatus.Migrating or CompanyStatus.Failed)
        {
            throw new InvalidOperationException(
                $"Cannot suspend a company in status '{company.Status}'.");
        }

        company.Status = CompanyStatus.Suspended;
        await catalog.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var company = await catalog.Companies.FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken)
            ?? throw new InvalidOperationException("Company not found.");

        if (company.Status == CompanyStatus.Active)
        {
            return;
        }

        if (company.Status is CompanyStatus.Migrating or CompanyStatus.Failed)
        {
            throw new InvalidOperationException(
                $"Cannot activate a company in status '{company.Status}'.");
        }

        company.Status = CompanyStatus.Active;
        await catalog.SaveChangesAsync(cancellationToken);
    }

    public async Task<TenantAccessCheckResult> CheckLoginAccessAsync(
        string companyKey,
        CancellationToken cancellationToken = default)
    {
        var key = companyKey.Trim().ToLowerInvariant();
        if (key.Length == 0)
        {
            return new TenantAccessCheckResult(false, "Company key is required.", null, null);
        }

        var company = await catalog.Companies.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyKey == key, cancellationToken);

        if (company is null)
        {
            return new TenantAccessCheckResult(false, "Unknown company key.", null, null);
        }

        return company.Status switch
        {
            CompanyStatus.Active => new TenantAccessCheckResult(true, null, company.Id, company.Status.ToString()),
            CompanyStatus.Suspended => new TenantAccessCheckResult(
                false,
                "Company is suspended. Contact platform support.",
                company.Id,
                company.Status.ToString()),
            CompanyStatus.Migrating => new TenantAccessCheckResult(
                false,
                "Company is migrating. Try again later.",
                company.Id,
                company.Status.ToString()),
            CompanyStatus.Failed => new TenantAccessCheckResult(
                false,
                "Company is unavailable.",
                company.Id,
                company.Status.ToString()),
            _ => new TenantAccessCheckResult(false, "Company is unavailable.", company.Id, company.Status.ToString())
        };
    }

    public async Task ResetAdminPasswordAsync(
        Guid companyId,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
        {
            throw new ArgumentException("New password must be at least 8 characters.");
        }

        var company = await catalog.Companies.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken)
            ?? throw new InvalidOperationException("Company not found.");

        if (company.Status == CompanyStatus.Suspended)
        {
            throw new InvalidOperationException("Cannot reset password for a suspended company.");
        }

        if (company.Status is CompanyStatus.Migrating or CompanyStatus.Failed)
        {
            throw new InvalidOperationException(
                $"Cannot reset password for a company in status '{company.Status}'.");
        }

        var cs = await connections.GetWriteConnectionStringAsync(companyId, cancellationToken);
        await using var tenant = tenantDbFactory.Create(cs);
        var admin = await tenant.Users
            .Where(x => x.Role == TenantRoles.Admin && x.IsActive)
            .OrderBy(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("No active Admin user found in tenant database.");

        admin.PasswordHash = passwordHasher.HashPassword(admin, newPassword);
        await tenant.SaveChangesAsync(cancellationToken);
    }

    private static TenantDetailDto MapDetail(Company company) =>
        new(
            company.Id,
            company.Name,
            company.CompanyKey,
            company.DatabaseName,
            company.Status.ToString(),
            company.LegacyMigrationStatus.ToString(),
            company.TimeZoneId,
            company.PlanId,
            company.Plan?.Name,
            company.CreatedAt,
            company.LastUsedAt,
            company.ActiveUsers30d,
            company.TotalUsersCached,
            company.SchemaVersion,
            company.DbCredentials
                .OrderBy(x => x.Role)
                .Select(x => new TenantCredentialSummaryDto(
                    x.Id,
                    x.Role.ToString(),
                    x.Host,
                    x.Port,
                    x.Username,
                    x.PasswordProtected.Length > 0,
                    x.IsActive,
                    x.SslMode,
                    x.RotatedAt))
                .ToList());
}
