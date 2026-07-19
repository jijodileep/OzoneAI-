using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OzoneAI.Application.Platform;
using OzoneAI.Application.Tenancy;
using OzoneAI.Domain.Catalog;
using OzoneAI.Infrastructure.FinancialYears;
using OzoneAI.Infrastructure.Persistence.Catalog;
using OzoneAI.Infrastructure.Persistence.Tenant;

namespace OzoneAI.Infrastructure.Tenancy;

public sealed class TenantMetricsService(
    CatalogDbContext catalog,
    ITenantConnectionFactory connectionFactory,
    ILogger<TenantMetricsService> logger) : ITenantMetricsService
{
    public async Task<TenantMetricsRefreshResult> RefreshAllAsync(CancellationToken cancellationToken = default)
    {
        var companies = await catalog.Companies
            .Where(x => x.Status != CompanyStatus.Failed)
            .ToListAsync(cancellationToken);

        var updated = 0;
        var failed = 0;
        var cutoff = DateTimeOffset.UtcNow.AddDays(-30);

        foreach (var company in companies)
        {
            try
            {
                var cs = await connectionFactory.GetConnectionStringAsync(
                    company.Id,
                    TenantDbCredentialRole.Write,
                    cancellationToken);

                var options = new DbContextOptionsBuilder<TenantDbContext>()
                    .UseNpgsql(cs)
                    .Options;

                await using var tenantDb = new TenantDbContext(options, new FinancialYearContext());

                var totalUsers = await tenantDb.Users.AsNoTracking().CountAsync(cancellationToken);
                var activeUsers = await tenantDb.Users.AsNoTracking()
                    .CountAsync(
                        x => x.IsActive && x.LastLoginAt != null && x.LastLoginAt >= cutoff,
                        cancellationToken);

                var lastLogin = await tenantDb.Users.AsNoTracking()
                    .Where(x => x.LastLoginAt != null)
                    .MaxAsync(x => (DateTimeOffset?)x.LastLoginAt, cancellationToken);

                company.TotalUsersCached = totalUsers;
                company.ActiveUsers30d = activeUsers;
                if (lastLogin is not null
                    && (company.LastUsedAt is null || lastLogin > company.LastUsedAt))
                {
                    company.LastUsedAt = lastLogin;
                }

                updated++;
            }
            catch (Exception ex)
            {
                failed++;
                logger.LogWarning(
                    ex,
                    "Failed to refresh metrics for company {CompanyKey}",
                    company.CompanyKey);
            }
        }

        await catalog.SaveChangesAsync(cancellationToken);
        return new TenantMetricsRefreshResult(updated, failed);
    }
}
