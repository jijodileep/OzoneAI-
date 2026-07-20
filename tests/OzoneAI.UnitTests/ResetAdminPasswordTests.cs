using OzoneAI.Domain.Catalog;
using OzoneAI.UnitTests.Support;

namespace OzoneAI.UnitTests;

public sealed class ResetAdminPasswordTests
{
    [Fact]
    public async Task ResetAdminPassword_updates_hash()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, admin) = await TestFixtures.SeedCompanyAsync(catalog, tf);
        var oldHash = admin.PasswordHash;
        var svc = TestFixtures.CreateLifecycle(catalog, tf);

        await svc.ResetAdminPasswordAsync(company.Id, "NewPass!123");

        await using var tenant = tf.Create("x");
        var updated = tenant.Users.Single(x => x.Id == admin.Id);
        Assert.NotEqual(oldHash, updated.PasswordHash);
        Assert.Equal(
            Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success,
            TestFixtures.Hasher.VerifyHashedPassword(updated, updated.PasswordHash, "NewPass!123"));
    }

    [Fact]
    public async Task ResetAdminPassword_rejects_suspended()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(
            catalog,
            tf,
            status: CompanyStatus.Suspended);
        var svc = TestFixtures.CreateLifecycle(catalog, tf);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.ResetAdminPasswordAsync(company.Id, "NewPass!123"));
    }

    [Fact]
    public async Task ResetAdminPassword_rejects_short_password()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, tf);
        var svc = TestFixtures.CreateLifecycle(catalog, tf);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.ResetAdminPasswordAsync(company.Id, "short"));
    }
}
