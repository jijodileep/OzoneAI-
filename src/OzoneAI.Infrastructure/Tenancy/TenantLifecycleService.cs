using Microsoft.EntityFrameworkCore;
using OzoneAI.Application.Platform;
using OzoneAI.Domain.Catalog;
using OzoneAI.Infrastructure.Persistence.Catalog;

namespace OzoneAI.Infrastructure.Tenancy;

public sealed class TenantLifecycleService(CatalogDbContext catalog) : ITenantLifecycleService
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
                    x.IsActive))
                .ToList());
}
