using OzoneAI.Domain.Catalog;
using OzoneAI.Infrastructure.Tenancy;
using OzoneAI.UnitTests.Support;

namespace OzoneAI.UnitTests;

public sealed class TenantConnectionFactoryTests
{
    [Fact]
    public async Task Write_and_Read_fallback_and_cache()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var factory = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, factory);
        var cache = TestFixtures.CreateCache();
        var connections = new TenantConnectionFactory(catalog, cache);

        var write = await connections.GetWriteConnectionStringAsync(company.Id);
        Assert.Contains(company.DatabaseName, write);
        Assert.Contains("localhost", write);

        var read = await connections.GetReadConnectionStringAsync(company.Id);
        Assert.Equal(write, read); // fallback

        var cached = await connections.GetWriteConnectionStringAsync(company.Id);
        Assert.Equal(write, cached);

        connections.InvalidateCache(company.Id);
        var again = await connections.GetConnectionStringAsync(company.Id, TenantDbCredentialRole.Write);
        Assert.Equal(write, again);
    }

    [Fact]
    public async Task Read_uses_dedicated_credential_when_present()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var factory = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, factory);
        catalog.TenantDbCredentials.Add(new TenantDbCredential
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Role = TenantDbCredentialRole.Read,
            Host = "replica",
            Port = 5432,
            Username = "reader",
            PasswordProtected = "rpw",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await catalog.SaveChangesAsync();

        var connections = new TenantConnectionFactory(catalog, TestFixtures.CreateCache());
        var read = await connections.GetReadConnectionStringAsync(company.Id);
        Assert.Contains("Host=replica", read);
        Assert.Contains("Username=reader", read);
    }

    [Fact]
    public async Task Missing_company_throws()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var connections = new TenantConnectionFactory(catalog, TestFixtures.CreateCache());
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => connections.GetWriteConnectionStringAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task SslMode_appended_to_connection_string()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var factory = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, factory);
        var cred = catalog.TenantDbCredentials.Single(c => c.CompanyId == company.Id);
        cred.SslMode = "Require";
        await catalog.SaveChangesAsync();

        var connections = new TenantConnectionFactory(catalog, TestFixtures.CreateCache());
        var write = await connections.GetWriteConnectionStringAsync(company.Id);
        Assert.Contains("SSL Mode=Require", write);
    }

    [Fact]
    public async Task Missing_write_credential_throws()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var id = Guid.NewGuid();
        catalog.Companies.Add(new Company
        {
            Id = id,
            Name = "x",
            CompanyKey = "x",
            DatabaseName = "ozone_t_x",
            Status = CompanyStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await catalog.SaveChangesAsync();
        var connections = new TenantConnectionFactory(catalog, TestFixtures.CreateCache());
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => connections.GetWriteConnectionStringAsync(id));
    }
}
