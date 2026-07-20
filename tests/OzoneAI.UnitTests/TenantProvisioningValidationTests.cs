using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using OzoneAI.Application.Platform;
using OzoneAI.Infrastructure.Tenancy;
using OzoneAI.UnitTests.Support;

namespace OzoneAI.UnitTests;

public sealed class TenantProvisioningValidationTests
{
    [Fact]
    public async Task External_mode_requires_connection_fields()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var svc = new TenantProvisioningService(
            catalog,
            new ConfigurationBuilder().Build(),
            new TenantDataSeeder(TestFixtures.Hasher),
            NullLogger<TenantProvisioningService>.Instance);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.CreateAsync(new CreateTenantRequest(
                Name: "Acme",
                CompanyKey: "acme",
                AdminUsername: "admin",
                AdminPassword: "ChangeMe!123",
                DbMode: TenantDbModes.External)));

        Assert.Contains("External mode requires", ex.Message);
    }

    [Fact]
    public async Task Invalid_dbMode_rejected()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var svc = new TenantProvisioningService(
            catalog,
            new ConfigurationBuilder().Build(),
            new TenantDataSeeder(TestFixtures.Hasher),
            NullLogger<TenantProvisioningService>.Instance);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.CreateAsync(new CreateTenantRequest(
                Name: "Acme",
                CompanyKey: "acme",
                AdminUsername: "admin",
                AdminPassword: "ChangeMe!123",
                DbMode: "SomethingElse")));
    }
}
