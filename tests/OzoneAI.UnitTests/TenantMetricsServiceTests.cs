using Microsoft.Extensions.Logging.Abstractions;
using OzoneAI.Domain.Catalog;
using OzoneAI.Infrastructure.Tenancy;
using OzoneAI.UnitTests.Support;

namespace OzoneAI.UnitTests;

public sealed class TenantMetricsServiceTests
{
    [Fact]
    public async Task RefreshAll_updates_counts_and_skips_failed()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, tf);

        catalog.Companies.Add(new Company
        {
            Id = Guid.NewGuid(),
            Name = "bad",
            CompanyKey = "bad",
            DatabaseName = "ozone_t_bad",
            Status = CompanyStatus.Failed,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await catalog.SaveChangesAsync();

        var svc = new TenantMetricsService(
            catalog,
            new TenantConnectionFactory(catalog, TestFixtures.CreateCache()),
            tf,
            NullLogger<TenantMetricsService>.Instance);

        var result = await svc.RefreshAllAsync();
        Assert.Equal(1, result.TenantsUpdated);
        Assert.Equal(0, result.TenantsFailed);

        var tracked = await catalog.Companies.FindAsync(company.Id);
        Assert.Equal(1, tracked!.TotalUsersCached);
    }

    [Fact]
    public async Task RefreshAll_counts_failures_when_connection_throws()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        catalog.Companies.Add(new Company
        {
            Id = Guid.NewGuid(),
            Name = "orphan",
            CompanyKey = "orphan",
            DatabaseName = "ozone_t_orphan",
            Status = CompanyStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await catalog.SaveChangesAsync();

        var svc = new TenantMetricsService(
            catalog,
            new TenantConnectionFactory(catalog, TestFixtures.CreateCache()),
            TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString()),
            NullLogger<TenantMetricsService>.Instance);

        var result = await svc.RefreshAllAsync();
        Assert.Equal(0, result.TenantsUpdated);
        Assert.Equal(1, result.TenantsFailed);
    }
}
