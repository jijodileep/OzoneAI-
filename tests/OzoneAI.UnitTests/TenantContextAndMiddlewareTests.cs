using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OzoneAI.Domain.Catalog;
using OzoneAI.Infrastructure.Auth;
using OzoneAI.Infrastructure.Tenancy;
using OzoneAI.UnitTests.Support;

namespace OzoneAI.UnitTests;

public sealed class TenantContextAndMiddlewareTests
{
    [Fact]
    public void TenantContext_Set_stores_values()
    {
        var ctx = new TenantContext();
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var imp = Guid.NewGuid();
        ctx.Set(companyId, "demo", "Host=x", TenantDbCredentialRole.Read, userId, "admin", "Admin", imp);
        Assert.Equal(companyId, ctx.CompanyId);
        Assert.Equal("demo", ctx.CompanyKey);
        Assert.Equal("Host=x", ctx.ConnectionString);
        Assert.Equal(TenantDbCredentialRole.Read, ctx.ConnectionRole);
        Assert.Equal(userId, ctx.UserId);
        Assert.Equal("admin", ctx.Username);
        Assert.Equal("Admin", ctx.Role);
        Assert.Equal(imp, ctx.ImpersonatedBy);
    }

    [Fact]
    public async Task Middleware_resolves_Read_for_GET_and_Write_for_POST()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, tf);
        var connections = new TenantConnectionFactory(catalog, TestFixtures.CreateCache());
        var middleware = new TenantResolutionMiddleware(_ => Task.CompletedTask);

        var tenantCtx = new TenantContext();
        var http = new DefaultHttpContext();
        http.Request.Method = HttpMethods.Get;
        http.User = TenantPrincipal(company.Id, company.CompanyKey, withImpersonation: true);

        await middleware.InvokeAsync(http, tenantCtx, connections);
        Assert.Equal(TenantDbCredentialRole.Read, tenantCtx.ConnectionRole);
        Assert.NotNull(tenantCtx.ConnectionString);
        Assert.NotNull(tenantCtx.ImpersonatedBy);

        var tenantCtx2 = new TenantContext();
        http.Request.Method = HttpMethods.Post;
        await middleware.InvokeAsync(http, tenantCtx2, connections);
        Assert.Equal(TenantDbCredentialRole.Write, tenantCtx2.ConnectionRole);
    }

    [Fact]
    public async Task Middleware_resolves_Read_for_HEAD()
    {
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var tf = TestFixtures.CreateTenantFactory(Guid.NewGuid().ToString());
        var (company, _, _) = await TestFixtures.SeedCompanyAsync(catalog, tf);
        var connections = new TenantConnectionFactory(catalog, TestFixtures.CreateCache());
        var middleware = new TenantResolutionMiddleware(_ => Task.CompletedTask);
        var tenantCtx = new TenantContext();
        var http = new DefaultHttpContext
        {
            Request = { Method = HttpMethods.Head },
            User = TenantPrincipal(company.Id, company.CompanyKey)
        };
        await middleware.InvokeAsync(http, tenantCtx, connections);
        Assert.Equal(TenantDbCredentialRole.Read, tenantCtx.ConnectionRole);
    }

    [Fact]
    public async Task Middleware_skips_non_tenant_scope_and_bad_claims()
    {
        var middleware = new TenantResolutionMiddleware(_ => Task.CompletedTask);
        await using var catalog = TestFixtures.CreateCatalog(Guid.NewGuid().ToString());
        var connections = new TenantConnectionFactory(catalog, TestFixtures.CreateCache());

        var platformCtx = new TenantContext();
        var platformHttp = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(JwtTokenService.AuthScopeClaim, JwtTokenService.PlatformScope)
            ], "test"))
        };
        await middleware.InvokeAsync(platformHttp, platformCtx, connections);
        Assert.Null(platformCtx.CompanyId);

        var badCtx = new TenantContext();
        var badHttp = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(JwtTokenService.AuthScopeClaim, JwtTokenService.TenantScope),
                new Claim(JwtTokenService.CompanyIdClaim, "not-a-guid"),
                new Claim(JwtTokenService.CompanyKeyClaim, "demo")
            ], "test"))
        };
        await middleware.InvokeAsync(badHttp, badCtx, connections);
        Assert.Null(badCtx.CompanyId);

        var emptyKeyCtx = new TenantContext();
        var emptyKeyHttp = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(JwtTokenService.AuthScopeClaim, JwtTokenService.TenantScope),
                new Claim(JwtTokenService.CompanyIdClaim, Guid.NewGuid().ToString()),
                new Claim(JwtTokenService.CompanyKeyClaim, " ")
            ], "test"))
        };
        await middleware.InvokeAsync(emptyKeyHttp, emptyKeyCtx, connections);
        Assert.Null(emptyKeyCtx.CompanyId);
    }

    [Fact]
    public void NpgsqlTenantDbContextFactory_creates_context()
    {
        var factory = new NpgsqlTenantDbContextFactory();
        using var db = factory.Create("Host=localhost;Port=5433;Database=ozone_t_demo;Username=ozone;Password=x");
        Assert.NotNull(db);
    }

    [Fact]
    public void JwtTokenService_issues_token_and_requires_key()
    {
        var jwt = TestFixtures.CreateJwt();
        var token = jwt.IssueToken([new Claim(ClaimTypes.Name, "a")], TimeSpan.FromMinutes(5));
        Assert.False(string.IsNullOrWhiteSpace(token));

        var defaultsOnly = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SigningKey"] = "OzoneAI-test-signing-key-32chars!!"
            })
            .Build();
        var withDefaults = new JwtTokenService(defaultsOnly);
        Assert.False(string.IsNullOrWhiteSpace(withDefaults.IssueToken([new Claim("a", "b")])));

        var empty = new ConfigurationBuilder().Build();
        var bad = new JwtTokenService(empty);
        Assert.Throws<InvalidOperationException>(() => bad.IssueToken([new Claim("a", "b")]));
    }

    private static ClaimsPrincipal TenantPrincipal(Guid companyId, string companyKey, bool withImpersonation = false)
    {
        var claims = new List<Claim>
        {
            new(JwtTokenService.AuthScopeClaim, JwtTokenService.TenantScope),
            new(JwtTokenService.CompanyIdClaim, companyId.ToString()),
            new(JwtTokenService.CompanyKeyClaim, companyKey),
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, "admin"),
            new(ClaimTypes.Role, "Admin")
        };
        if (withImpersonation)
        {
            claims.Add(new Claim(JwtTokenService.ImpersonatedByClaim, Guid.NewGuid().ToString()));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
    }
}
