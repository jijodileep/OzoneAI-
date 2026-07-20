using Microsoft.AspNetCore.Identity;
using OzoneAI.Application.Platform;
using OzoneAI.Domain.Catalog;
using OzoneAI.Domain.Tenant;
using OzoneAI.Infrastructure.Auth;
using OzoneAI.Infrastructure.Persistence.Catalog;
using OzoneAI.Infrastructure.Tenancy;
using OzoneAI.UnitTests.Support;

namespace OzoneAI.UnitTests;

public sealed class TenantAuthServiceTests
{
    [Theory]
    [InlineData("", "admin", "x")]
    [InlineData("demo", "", "x")]
    [InlineData("demo", "admin", "")]
    public async Task Login_rejects_empty_inputs(string key, string user, string pass)
    {
        var svc = CreateEmpty();
        Assert.Null(await svc.LoginAsync(key, user, pass));
    }

    [Fact]
    public async Task Login_rejects_suspended_company()
    {
        var (svc, _, _, _) = await CreateSeededAsync(CompanyStatus.Suspended);
        Assert.Null(await svc.LoginAsync("demo", "admin", "ChangeMe!123"));
    }

    [Fact]
    public async Task Login_rejects_bad_password_and_inactive_user()
    {
        var (svc, _, tenantFactory, _) = await CreateSeededAsync();
        Assert.Null(await svc.LoginAsync("demo", "admin", "wrong"));

        await using var tenant = tenantFactory.Create("x");
        var u = tenant.Users.First();
        u.IsActive = false;
        await tenant.SaveChangesAsync();
        Assert.Null(await svc.LoginAsync("demo", "admin", "ChangeMe!123"));
    }

    [Fact]
    public async Task Login_rejects_unknown_username()
    {
        var (svc, _, _, _) = await CreateSeededAsync();
        Assert.Null(await svc.LoginAsync("demo", "nobody", "ChangeMe!123"));
    }

    [Fact]
    public async Task Login_success_updates_last_login_and_issues_token()
    {
        var (svc, catalog, _, company) = await CreateSeededAsync();
        var result = await svc.LoginAsync("DEMO", "admin", "ChangeMe!123");
        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result!.AccessToken));
        Assert.Equal("demo", result.Company.CompanyKey);
        Assert.Null(result.ImpersonatedBy);

        var tracked = await catalog.Companies.FindAsync(company.Id);
        Assert.NotNull(tracked!.LastUsedAt);
    }

    [Fact]
    public async Task Login_returns_null_when_company_row_missing_after_access()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var missingId = Guid.NewGuid();
        var lifecycle = new StubLifecycle(new TenantAccessCheckResult(true, null, missingId, "Active"));
        var svc = new TenantAuthService(
            catalog,
            lifecycle,
            new TenantConnectionFactory(catalog, TestFixtures.CreateCache()),
            tf,
            TestFixtures.CreateJwt(),
            TestFixtures.Hasher);

        Assert.Null(await svc.LoginAsync("ghost", "admin", "ChangeMe!123"));
    }

    [Fact]
    public async Task IssueTokenForUser_success_and_failures()
    {
        var (svc, _, tenantFactory, company) = await CreateSeededAsync();
        await using var tenant = tenantFactory.Create("x");
        var adminId = tenant.Users.First().Id;

        var ok = await svc.IssueTokenForUserAsync(company.Id, adminId, TimeSpan.FromMinutes(30), Guid.NewGuid());
        Assert.NotNull(ok.ImpersonatedBy);
        Assert.Equal(1800, ok.ExpiresInSeconds);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => svc.IssueTokenForUserAsync(Guid.NewGuid(), adminId, TimeSpan.FromMinutes(1)));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => svc.IssueTokenForUserAsync(company.Id, Guid.NewGuid(), TimeSpan.FromMinutes(1)));
    }

    [Fact]
    public async Task IssueTokenForUser_rejects_suspended()
    {
        var (svc, catalog, tenantFactory, company) = await CreateSeededAsync();
        company.Status = CompanyStatus.Suspended;
        await catalog.SaveChangesAsync();
        await using var tenant = tenantFactory.Create("x");
        var adminId = tenant.Users.First().Id;
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => svc.IssueTokenForUserAsync(company.Id, adminId, TimeSpan.FromMinutes(1)));
    }

    [Fact]
    public async Task Login_rehashes_when_needed()
    {
        var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, admin) = await TestFixtures.SeedCompanyAsync(catalog, tf);
        var rehashHasher = new RehashHasher();
        admin.PasswordHash = rehashHasher.HashPassword(admin, "ChangeMe!123");
        await using (var t = tf.Create("x"))
        {
            var existing = t.Users.First();
            existing.PasswordHash = admin.PasswordHash;
            await t.SaveChangesAsync();
        }

        var auth = new TenantAuthService(
            catalog,
            new TenantLifecycleService(catalog),
            new TenantConnectionFactory(catalog, TestFixtures.CreateCache()),
            tf,
            TestFixtures.CreateJwt(),
            rehashHasher);

        var result = await auth.LoginAsync(company.CompanyKey, "admin", "ChangeMe!123");
        Assert.NotNull(result);
        Assert.True(rehashHasher.Rehashed);
    }

    private static TenantAuthService CreateEmpty()
    {
        var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        return new TenantAuthService(
            catalog,
            new TenantLifecycleService(catalog),
            new TenantConnectionFactory(catalog, TestFixtures.CreateCache()),
            tf,
            TestFixtures.CreateJwt(),
            TestFixtures.Hasher);
    }

    private static async Task<(TenantAuthService svc, CatalogDbContext catalog, InMemoryTenantDbContextFactory tf, Company company)> CreateSeededAsync(
        CompanyStatus status = CompanyStatus.Active)
    {
        var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, tf, status: status);
        var svc = new TenantAuthService(
            catalog,
            new TenantLifecycleService(catalog),
            new TenantConnectionFactory(catalog, TestFixtures.CreateCache()),
            tf,
            TestFixtures.CreateJwt(),
            TestFixtures.Hasher);
        return (svc, catalog, tf, company);
    }

    private sealed class StubLifecycle(TenantAccessCheckResult result) : ITenantLifecycleService
    {
        public Task ActivateAsync(Guid companyId, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<TenantAccessCheckResult> CheckLoginAccessAsync(
            string companyKey,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);

        public Task<TenantDetailDto?> GetAsync(Guid companyId, CancellationToken cancellationToken = default) =>
            Task.FromResult<TenantDetailDto?>(null);

        public Task SuspendAsync(Guid companyId, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class RehashHasher : IPasswordHasher<TenantUser>
    {
        private readonly PasswordHasher<TenantUser> _inner = new();
        public bool Rehashed { get; private set; }

        public string HashPassword(TenantUser user, string password) => _inner.HashPassword(user, password);

        public PasswordVerificationResult VerifyHashedPassword(
            TenantUser user,
            string hashedPassword,
            string providedPassword)
        {
            var r = _inner.VerifyHashedPassword(user, hashedPassword, providedPassword);
            if (r == PasswordVerificationResult.Success)
            {
                Rehashed = true;
                return PasswordVerificationResult.SuccessRehashNeeded;
            }

            return r;
        }
    }
}
