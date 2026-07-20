using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using OzoneAI.Domain.Catalog;
using OzoneAI.Domain.Tenant;
using OzoneAI.Infrastructure.Auth;
using OzoneAI.Infrastructure.FinancialYears;
using OzoneAI.Infrastructure.Persistence.Catalog;
using OzoneAI.Infrastructure.Persistence.Tenant;
using OzoneAI.Infrastructure.Tenancy;

namespace OzoneAI.UnitTests.Support;

internal static class TestFixtures
{
    public static CatalogDbContext CreateCatalog(string name)
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new CatalogDbContext(options);
    }

    public static InMemoryTenantDbContextFactory CreateTenantFactory(string name) => new(name);

    public static IMemoryCache CreateCache() => new MemoryCache(new MemoryCacheOptions());

    public static JwtTokenService CreateJwt()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SigningKey"] = "OzoneAI-test-signing-key-32chars!!",
                ["Jwt:Issuer"] = "OzoneAI",
                ["Jwt:Audience"] = "OzoneAI"
            })
            .Build();
        return new JwtTokenService(config);
    }

    public static PasswordHasher<TenantUser> Hasher { get; } = new();

    public static async Task<(Company company, PlatformUser platform, TenantUser admin)> SeedCompanyAsync(
        CatalogDbContext catalog,
        InMemoryTenantDbContextFactory tenantFactory,
        string key = "demo",
        CompanyStatus status = CompanyStatus.Active)
    {
        var companyId = Guid.NewGuid();
        var company = new Company
        {
            Id = companyId,
            Name = $"{key} Co",
            CompanyKey = key,
            DatabaseName = $"ozone_t_{key}",
            Status = status,
            LegacyMigrationStatus = LegacyMigrationStatus.NotStarted,
            TimeZoneId = "Asia/Kolkata",
            CreatedAt = DateTimeOffset.UtcNow,
            TotalUsersCached = 1
        };
        catalog.Companies.Add(company);
        catalog.TenantDbCredentials.Add(new TenantDbCredential
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Role = TenantDbCredentialRole.Write,
            Host = "localhost",
            Port = 5433,
            Username = "ozone",
            PasswordProtected = "secret",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        });

        var platform = new PlatformUser
        {
            Id = Guid.NewGuid(),
            Username = "superadmin",
            DisplayName = "SA",
            PasswordHash = "x",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
        catalog.PlatformUsers.Add(platform);
        await catalog.SaveChangesAsync();

        await using var tenant = tenantFactory.Create("ignored");
        var admin = new TenantUser
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            DisplayName = "Admin",
            Role = TenantRoles.Admin,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
        admin.PasswordHash = Hasher.HashPassword(admin, "ChangeMe!123");
        tenant.Users.Add(admin);
        await tenant.SaveChangesAsync();

        return (company, platform, admin);
    }
}

internal sealed class InMemoryTenantDbContextFactory(string databaseName) : ITenantDbContextFactory
{
    public TenantDbContext Create(string connectionString)
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
        return new TenantDbContext(options, new FinancialYearContext());
    }
}
