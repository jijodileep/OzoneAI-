using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using OzoneAI.Domain.Catalog;
using OzoneAI.Infrastructure.Auth;
using OzoneAI.Infrastructure.Tenancy;
using OzoneAI.UnitTests.Support;

namespace OzoneAI.UnitTests;

public sealed class PasswordResetServiceTests
{
    [Fact]
    public async Task Tenant_forgot_and_reset_round_trip()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, admin) = await TestFixtures.SeedCompanyAsync(catalog, tf);
        var email = new CapturingEmailSender();
        var svc = CreateService(catalog, tf, email);

        await svc.RequestTenantResetAsync(company.CompanyKey, admin.Email!);
        Assert.Single(email.Sent);
        var link = email.Sent[0].PlainTextBody;
        var token = ExtractQuery(link, "token");
        Assert.False(string.IsNullOrEmpty(token));

        await svc.ResetTenantPasswordAsync(company.CompanyKey, token!, "ResetPass!1");

        await using var tenant = tf.Create("x");
        var updated = await tenant.Users.SingleAsync(x => x.Id == admin.Id);
        Assert.Equal(
            PasswordVerificationResult.Success,
            TestFixtures.Hasher.VerifyHashedPassword(updated, updated.PasswordHash, "ResetPass!1"));
    }

    [Fact]
    public async Task Tenant_forgot_unknown_user_is_silent()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, tf);
        var email = new CapturingEmailSender();
        var svc = CreateService(catalog, tf, email);

        await svc.RequestTenantResetAsync(company.CompanyKey, "nobody@example.com");
        Assert.Empty(email.Sent);
    }

    [Fact]
    public async Task Platform_forgot_and_reset_round_trip()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var hasher = new PasswordHasher<PlatformUser>();
        var platform = new PlatformUser
        {
            Id = Guid.NewGuid(),
            Username = "superadmin",
            DisplayName = "SA",
            Email = "sa@example.com",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
        platform.PasswordHash = hasher.HashPassword(platform, "OldPass!123");
        catalog.PlatformUsers.Add(platform);
        await catalog.SaveChangesAsync();

        var email = new CapturingEmailSender();
        var svc = CreateService(catalog, tf, email, hasher);

        await svc.RequestPlatformResetAsync("sa@example.com");
        Assert.Single(email.Sent);
        var token = ExtractQuery(email.Sent[0].PlainTextBody, "token");
        Assert.False(string.IsNullOrEmpty(token));

        await svc.ResetPlatformPasswordAsync(token!, "NewSa!12345");

        var updated = await catalog.PlatformUsers.SingleAsync(x => x.Id == platform.Id);
        Assert.Equal(
            PasswordVerificationResult.Success,
            hasher.VerifyHashedPassword(updated, updated.PasswordHash, "NewSa!12345"));
    }

    [Fact]
    public async Task Reset_rejects_bad_token()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, tf);
        var svc = CreateService(catalog, tf, new CapturingEmailSender());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.ResetTenantPasswordAsync(company.CompanyKey, "not-a-real-token", "ResetPass!1"));
    }

    private static PasswordResetService CreateService(
        OzoneAI.Infrastructure.Persistence.Catalog.CatalogDbContext catalog,
        InMemoryTenantDbContextFactory tf,
        CapturingEmailSender email,
        PasswordHasher<PlatformUser>? platformHasher = null)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["App:PublicWebBaseUrl"] = "http://localhost:5173"
            })
            .Build();

        return new PasswordResetService(
            catalog,
            TestFixtures.CreateLifecycle(catalog, tf),
            new TenantConnectionFactory(catalog, TestFixtures.CreateCache()),
            tf,
            TestFixtures.Hasher,
            platformHasher ?? new PasswordHasher<PlatformUser>(),
            email,
            config,
            NullLogger<PasswordResetService>.Instance);
    }

    private static string? ExtractQuery(string body, string key)
    {
        var marker = $"{key}=";
        var idx = body.IndexOf(marker, StringComparison.Ordinal);
        if (idx < 0) return null;
        var start = idx + marker.Length;
        var end = start;
        while (end < body.Length && !char.IsWhiteSpace(body[end]) && body[end] != '\n')
        {
            end++;
        }

        return Uri.UnescapeDataString(body[start..end].TrimEnd('.', ')', ']'));
    }
}
