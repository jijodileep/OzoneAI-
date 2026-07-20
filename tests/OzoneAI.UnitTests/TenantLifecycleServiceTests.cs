using OzoneAI.Domain.Catalog;
using OzoneAI.Infrastructure.Tenancy;
using OzoneAI.UnitTests.Support;

namespace OzoneAI.UnitTests;

public sealed class TenantLifecycleServiceTests
{
    [Fact]
    public async Task GetAsync_returns_null_when_missing()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var svc = new TenantLifecycleService(catalog);
        Assert.Null(await svc.GetAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAsync_maps_detail_with_credentials()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var factory = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, factory);
        var svc = new TenantLifecycleService(catalog);
        var detail = await svc.GetAsync(company.Id);
        Assert.NotNull(detail);
        Assert.Equal(company.CompanyKey, detail!.CompanyKey);
        Assert.Null(detail.PlanName);
        Assert.Single(detail.Credentials);
        Assert.True(detail.Credentials[0].PasswordSet);
    }

    [Fact]
    public async Task GetAsync_maps_plan_and_empty_password_flag()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var factory = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, factory, key: "withplan");
        var plan = new SubscriptionPlan
        {
            Id = Guid.NewGuid(),
            Name = "Pro",
            MaxUsers = 10,
            CreatedAt = DateTimeOffset.UtcNow
        };
        catalog.SubscriptionPlans.Add(plan);
        company.PlanId = plan.Id;
        catalog.TenantDbCredentials.Add(new TenantDbCredential
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Role = TenantDbCredentialRole.Read,
            Host = "replica",
            Port = 5432,
            Username = "ro",
            PasswordProtected = "",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await catalog.SaveChangesAsync();

        var svc = new TenantLifecycleService(catalog);
        var detail = await svc.GetAsync(company.Id);
        Assert.NotNull(detail);
        Assert.Equal("Pro", detail!.PlanName);
        Assert.Equal(2, detail.Credentials.Count);
        Assert.Contains(detail.Credentials, c => !c.PasswordSet);
    }

    [Fact]
    public async Task Suspend_and_Activate_round_trip()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var factory = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, factory);
        var svc = new TenantLifecycleService(catalog);

        await svc.SuspendAsync(company.Id);
        Assert.Equal(CompanyStatus.Suspended, (await catalog.Companies.FindAsync(company.Id))!.Status);

        await svc.SuspendAsync(company.Id); // idempotent
        await svc.ActivateAsync(company.Id);
        Assert.Equal(CompanyStatus.Active, (await catalog.Companies.FindAsync(company.Id))!.Status);
        await svc.ActivateAsync(company.Id); // idempotent
    }

    [Fact]
    public async Task Suspend_rejects_Migrating()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var factory = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, factory, status: CompanyStatus.Migrating);
        var svc = new TenantLifecycleService(catalog);
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.SuspendAsync(company.Id));
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.ActivateAsync(company.Id));
    }

    [Fact]
    public async Task Suspend_missing_company_throws()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var svc = new TenantLifecycleService(catalog);
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.SuspendAsync(Guid.NewGuid()));
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.ActivateAsync(Guid.NewGuid()));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CheckLoginAccess_rejects_empty_key(string key)
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var svc = new TenantLifecycleService(catalog);
        var result = await svc.CheckLoginAccessAsync(key);
        Assert.False(result.Allowed);
    }

    [Fact]
    public async Task CheckLoginAccess_unknown_key()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var svc = new TenantLifecycleService(catalog);
        var result = await svc.CheckLoginAccessAsync("nope");
        Assert.False(result.Allowed);
        Assert.Equal("Unknown company key.", result.Reason);
    }

    [Theory]
    [InlineData(CompanyStatus.Active, true)]
    [InlineData(CompanyStatus.Suspended, false)]
    [InlineData(CompanyStatus.Migrating, false)]
    [InlineData(CompanyStatus.Failed, false)]
    public async Task CheckLoginAccess_by_status(CompanyStatus status, bool allowed)
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var factory = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var key = $"k{status}".ToLowerInvariant();
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, factory, key: key, status: status);
        var svc = new TenantLifecycleService(catalog);
        var result = await svc.CheckLoginAccessAsync(company.CompanyKey.ToUpperInvariant());
        Assert.Equal(allowed, result.Allowed);
        Assert.Equal(company.Id, result.CompanyId);
    }
}
