using OzoneAI.Application.Auth;
using OzoneAI.Application.Platform;
using OzoneAI.Domain.Catalog;
using OzoneAI.Infrastructure.Auth;
using OzoneAI.Infrastructure.Persistence.Catalog;
using OzoneAI.Infrastructure.Platform;
using OzoneAI.Infrastructure.Tenancy;
using OzoneAI.UnitTests.Support;

namespace OzoneAI.UnitTests;

public sealed class ImpersonationServiceTests
{
    [Theory]
    [InlineData("ab")]
    [InlineData("")]
    public async Task Impersonate_rejects_short_reason(string reason)
    {
        var (svc, company, platformId) = await CreateAsync();
        await Assert.ThrowsAsync<ArgumentException>(
            () => svc.ImpersonateAsync(company.Id, platformId, reason, null));
    }

    [Fact]
    public async Task Impersonate_rejects_long_reason()
    {
        var (svc, company, platformId) = await CreateAsync();
        await Assert.ThrowsAsync<ArgumentException>(
            () => svc.ImpersonateAsync(company.Id, platformId, new string('x', 501), null));
    }

    [Fact]
    public async Task Impersonate_success_writes_audit()
    {
        var (svc, company, platformId) = await CreateAsync();
        var result = await svc.ImpersonateAsync(company.Id, platformId, "Support ticket 1", "127.0.0.1");
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.Equal(platformId, result.ImpersonatedBy);
        Assert.Equal(3600, result.ExpiresInSeconds);

        var audits = await svc.ListAuditsAsync(company.Id, take: 5);
        Assert.Single(audits);
        Assert.Equal("Support ticket 1", audits[0].Reason);
        Assert.Equal("superadmin", audits[0].PlatformUsername);
    }

    [Fact]
    public async Task Impersonate_not_found_and_suspended()
    {
        var (svc, company, platformId, catalog) = await CreateFullAsync();
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => svc.ImpersonateAsync(Guid.NewGuid(), platformId, "reason ok", null));

        company.Status = CompanyStatus.Suspended;
        await catalog.SaveChangesAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => svc.ImpersonateAsync(company.Id, platformId, "reason ok", null));
    }

    [Fact]
    public async Task Impersonate_rejects_inactive_platform_user()
    {
        var (svc, company, platformId, catalog) = await CreateFullAsync();
        var pu = await catalog.PlatformUsers.FindAsync(platformId);
        pu!.IsActive = false;
        await catalog.SaveChangesAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => svc.ImpersonateAsync(company.Id, platformId, "reason ok", null));
    }

    [Fact]
    public async Task Impersonate_rejects_when_no_tenant_users()
    {
        var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, platform, _) = await TestFixtures.SeedCompanyAsync(catalog, tf);
        await using (var t = tf.Create("x"))
        {
            t.Users.RemoveRange(t.Users);
            await t.SaveChangesAsync();
        }

        var auth = new TenantAuthService(
            catalog,
            new TenantLifecycleService(catalog),
            new TenantConnectionFactory(catalog, TestFixtures.CreateCache()),
            tf,
            TestFixtures.CreateJwt(),
            TestFixtures.Hasher);
        var svc = new ImpersonationService(
            catalog,
            new TenantLifecycleService(catalog),
            new TenantConnectionFactory(catalog, TestFixtures.CreateCache()),
            tf,
            auth);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => svc.ImpersonateAsync(company.Id, platform.Id, "reason ok", null));
    }

    [Fact]
    public async Task ListAudits_clamps_take()
    {
        var (svc, company, platformId) = await CreateAsync();
        await svc.ImpersonateAsync(company.Id, platformId, "reason ok", null);
        var audits = await svc.ListAuditsAsync(company.Id, take: 0);
        Assert.Single(audits);
        var many = await svc.ListAuditsAsync(company.Id, take: 1000);
        Assert.Single(many);
    }

    private static async Task<(IImpersonationService svc, Company company, Guid platformId)> CreateAsync()
    {
        var (svc, company, platformId, _) = await CreateFullAsync();
        return (svc, company, platformId);
    }

    private static async Task<(ImpersonationService svc, Company company, Guid platformId, CatalogDbContext catalog)> CreateFullAsync()
    {
        var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, platform, _) = await TestFixtures.SeedCompanyAsync(catalog, tf);
        var auth = new TenantAuthService(
            catalog,
            new TenantLifecycleService(catalog),
            new TenantConnectionFactory(catalog, TestFixtures.CreateCache()),
            tf,
            TestFixtures.CreateJwt(),
            TestFixtures.Hasher);
        var svc = new ImpersonationService(
            catalog,
            new TenantLifecycleService(catalog),
            new TenantConnectionFactory(catalog, TestFixtures.CreateCache()),
            tf,
            auth);
        return (svc, company, platform.Id, catalog);
    }
}
