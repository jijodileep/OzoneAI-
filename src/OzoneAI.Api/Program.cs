using System.Security.Claims;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneAI.Application.FinancialYears;
using OzoneAI.Application.Platform;
using OzoneAI.Domain.Catalog;
using OzoneAI.Domain.Tenant;
using OzoneAI.Infrastructure;
using OzoneAI.Infrastructure.Auth;
using OzoneAI.Infrastructure.Persistence.Catalog;
using OzoneAI.Infrastructure.Persistence.Tenant;
using OzoneAI.Infrastructure.Tenancy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<CatalogDbContext>("catalog-db")
    .AddDbContextCheck<TenantDbContext>("tenant-db");
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var catalog = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await catalog.Database.MigrateAsync();
    await SeedCatalogAsync(catalog, scope.ServiceProvider);

    var tenant = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
    await tenant.Database.MigrateAsync();
    var seeder = scope.ServiceProvider.GetRequiredService<TenantDataSeeder>();
    await seeder.SeedDefaultsAsync(
        tenant,
        new TenantSeedOptions(
            LegalName: "Demo Company",
            Address: "Demo Address",
            Phone: "0000000000",
            Email: null,
            TaxType: "GST",
            CurrencyCode: "INR",
            AdminUsername: "admin",
            AdminPassword: "ChangeMe!123",
            AdminDisplayName: "Demo Admin"));
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new LocalRequestsOnlyAuthorizationFilter()]
});

RecurringJob.AddOrUpdate<TenantMetricsRollupJob>(
    "tenant-metrics-rollup",
    job => job.ExecuteAsync(CancellationToken.None),
    Cron.Daily(2));

app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Ok(new
{
    service = "OzoneAI.Api",
    status = "ok"
}));

app.MapPost("/v1/platform/auth/login", async (
    PlatformLoginRequest body,
    IPlatformAuthService auth,
    CancellationToken ct) =>
{
    var result = await auth.LoginAsync(body.Username, body.Password, ct);
    return result is null
        ? Results.Unauthorized()
        : Results.Ok(result);
}).AllowAnonymous();

app.MapGet("/v1/platform/me", (ClaimsPrincipal user) =>
{
    var id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
    return Results.Ok(new
    {
        id,
        username = user.Identity?.Name,
        role = user.FindFirstValue(ClaimTypes.Role),
        scope = user.FindFirstValue(JwtTokenService.AuthScopeClaim)
    });
}).RequireAuthorization("SuperAdminOnly");

app.MapGet("/v1/platform/tenants", async (
    ITenantProvisioningService provisioning,
    CancellationToken ct) =>
{
    var tenants = await provisioning.ListAsync(ct);
    return Results.Ok(tenants);
}).RequireAuthorization("SuperAdminOnly");

app.MapPost("/v1/platform/tenants", async (
    CreateTenantRequest body,
    ITenantProvisioningService provisioning,
    CancellationToken ct) =>
{
    try
    {
        var result = await provisioning.CreateAsync(body, ct);
        return Results.Created($"/v1/platform/tenants/{result.CompanyId}", result);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            title: "Tenant provisioning failed",
            statusCode: StatusCodes.Status500InternalServerError);
    }
}).RequireAuthorization("SuperAdminOnly");

app.MapPost("/v1/platform/tenants/metrics/refresh", async (
    ITenantMetricsService metrics,
    CancellationToken ct) =>
{
    var result = await metrics.RefreshAllAsync(ct);
    return Results.Ok(result);
}).RequireAuthorization("SuperAdminOnly");

app.MapGet("/v1/platform/tenants/{companyId:guid}", async (
    Guid companyId,
    ITenantLifecycleService lifecycle,
    CancellationToken ct) =>
{
    var detail = await lifecycle.GetAsync(companyId, ct);
    return detail is null ? Results.NotFound() : Results.Ok(detail);
}).RequireAuthorization("SuperAdminOnly");

app.MapPost("/v1/platform/tenants/{companyId:guid}/suspend", async (
    Guid companyId,
    ITenantLifecycleService lifecycle,
    CancellationToken ct) =>
{
    try
    {
        await lifecycle.SuspendAsync(companyId, ct);
        var detail = await lifecycle.GetAsync(companyId, ct);
        return Results.Ok(detail);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).RequireAuthorization("SuperAdminOnly");

app.MapPost("/v1/platform/tenants/{companyId:guid}/activate", async (
    Guid companyId,
    ITenantLifecycleService lifecycle,
    CancellationToken ct) =>
{
    try
    {
        await lifecycle.ActivateAsync(companyId, ct);
        var detail = await lifecycle.GetAsync(companyId, ct);
        return Results.Ok(detail);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).RequireAuthorization("SuperAdminOnly");

/// <summary>Preflight for company-key login (E3.1). Suspended tenants are rejected here.</summary>
app.MapPost("/v1/auth/company-access", async (
    CompanyAccessRequest body,
    ITenantLifecycleService lifecycle,
    CancellationToken ct) =>
{
    var result = await lifecycle.CheckLoginAccessAsync(body.CompanyKey, ct);
    return result.Allowed
        ? Results.Ok(result)
        : Results.Json(result, statusCode: StatusCodes.Status403Forbidden);
}).AllowAnonymous();

app.MapGet("/catalog/companies/count", async (CatalogDbContext db, CancellationToken ct) =>
{
    var count = await db.Companies.CountAsync(ct);
    return Results.Ok(new { companies = count });
}).RequireAuthorization("SuperAdminOnly");

app.MapGet("/catalog/plans", async (CatalogDbContext db, CancellationToken ct) =>
{
    var plans = await db.SubscriptionPlans.AsNoTracking()
        .OrderBy(x => x.Name)
        .Select(x => new { x.Id, x.Name, x.MaxUsers, x.MaxGodowns, x.IsActive })
        .ToListAsync(ct);
    return Results.Ok(plans);
}).RequireAuthorization("SuperAdminOnly");

app.MapGet("/catalog/companies/{companyId:guid}/db-credentials", async (
    Guid companyId,
    CatalogDbContext db,
    CancellationToken ct) =>
{
    var rows = await db.TenantDbCredentials.AsNoTracking()
        .Where(x => x.CompanyId == companyId && x.IsActive)
        .Select(x => new
        {
            x.Id,
            Role = x.Role.ToString(),
            x.Host,
            x.Port,
            x.Username,
            PasswordSet = x.PasswordProtected.Length > 0,
            x.RotatedAt
        })
        .ToListAsync(ct);
    return Results.Ok(rows);
}).RequireAuthorization("SuperAdminOnly");

app.MapGet("/v1/financial-years", async (TenantDbContext db, CancellationToken ct) =>
{
    var years = await db.FinancialYears.AsNoTracking()
        .OrderByDescending(x => x.StartDate)
        .Select(x => new { x.Id, x.Name, x.StartDate, x.EndDate, Status = x.Status.ToString(), x.IsDefault })
        .ToListAsync(ct);
    return Results.Ok(years);
});

app.MapPost("/v1/session/financial-year", async (
    SwitchFyRequest body,
    IFinancialYearSwitchService switchService,
    CancellationToken ct) =>
{
    var result = await switchService.SwitchAsync(body.FinancialYearId, ct);
    return Results.Ok(result);
});

app.MapGet("/v1/reports/balance-sheet", async (
    Guid financialYearId,
    DateOnly? asOf,
    IBalanceSheetService balanceSheet,
    CancellationToken ct) =>
{
    var date = asOf ?? DateOnly.FromDateTime(DateTime.UtcNow);
    var result = await balanceSheet.BuildAsync(financialYearId, date, ct);
    return Results.Ok(result);
});

app.MapPost("/v1/financial-years/{id:guid}/close", async (
    Guid id,
    IYearCloseService yearClose,
    CancellationToken ct) =>
{
    var nextId = await yearClose.CloseYearAsync(id, closedByUserId: null, ct);
    return Results.Ok(new { nextFinancialYearId = nextId });
});

app.MapGet("/v1/company/profile", async (TenantDbContext db, CancellationToken ct) =>
{
    var profile = await db.CompanyProfiles.AsNoTracking().FirstOrDefaultAsync(ct);
    return profile is null ? Results.NotFound() : Results.Ok(profile);
});

app.Run();

static async Task SeedCatalogAsync(CatalogDbContext catalog, IServiceProvider services)
{
    if (!await catalog.SubscriptionPlans.AnyAsync())
    {
        catalog.SubscriptionPlans.Add(new SubscriptionPlan
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "General",
            Details = "Default OzoneAI plan",
            MaxUsers = 10,
            MaxGodowns = 3,
            ModuleFlagsJson = """{"crm":true,"pos":true}""",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await catalog.SaveChangesAsync();
    }

    var config = services.GetRequiredService<IConfiguration>();
    var username = config["PlatformAuth:SeedUsername"] ?? "superadmin";
    if (!await catalog.PlatformUsers.AnyAsync(x => x.Username == username))
    {
        var hasher = services.GetRequiredService<IPasswordHasher<PlatformUser>>();
        var password = config["PlatformAuth:SeedPassword"] ?? "ChangeMe!123";
        var user = new PlatformUser
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Username = username,
            DisplayName = config["PlatformAuth:SeedDisplayName"] ?? "Platform Super Admin",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
        user.PasswordHash = hasher.HashPassword(user, password);
        catalog.PlatformUsers.Add(user);
        await catalog.SaveChangesAsync();
    }

    if (!await catalog.Companies.AnyAsync(x => x.CompanyKey == "demo"))
    {
        var planId = await catalog.SubscriptionPlans.AsNoTracking()
            .Where(x => x.IsActive)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync();

        var (host, port, dbUser, dbPassword) = TenantDbEndpoint.Resolve(config);

        var companyId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        catalog.Companies.Add(new Company
        {
            Id = companyId,
            Name = "Demo Company",
            CompanyKey = "demo",
            DatabaseName = "ozone_t_demo",
            Status = CompanyStatus.Active,
            LegacyMigrationStatus = LegacyMigrationStatus.NotStarted,
            TimeZoneId = "Asia/Kolkata",
            PlanId = planId,
            CreatedAt = DateTimeOffset.UtcNow,
            TotalUsersCached = 1
        });
        catalog.TenantDbCredentials.Add(new TenantDbCredential
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Role = TenantDbCredentialRole.Write,
            Host = host,
            Port = port,
            Username = dbUser,
            PasswordProtected = dbPassword,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await catalog.SaveChangesAsync();
    }
    else
    {
        // Heal demo credentials when switching between Docker/local Catalog hosts.
        var (host, port, dbUser, dbPassword) = TenantDbEndpoint.Resolve(config);
        var demo = await catalog.Companies.AsNoTracking()
            .FirstAsync(x => x.CompanyKey == "demo");
        var creds = await catalog.TenantDbCredentials
            .Where(x => x.CompanyId == demo.Id && x.IsActive)
            .ToListAsync();
        var changed = false;
        foreach (var cred in creds)
        {
            if (cred.Host == host && cred.Port == port
                && cred.Username == dbUser && cred.PasswordProtected == dbPassword)
            {
                continue;
            }

            cred.Host = host;
            cred.Port = port;
            cred.Username = dbUser;
            cred.PasswordProtected = dbPassword;
            cred.RotatedAt = DateTimeOffset.UtcNow;
            changed = true;
        }

        if (changed)
        {
            await catalog.SaveChangesAsync();
        }
    }
}

internal sealed record SwitchFyRequest(Guid FinancialYearId);

internal sealed record PlatformLoginRequest(string Username, string Password);

internal sealed record CompanyAccessRequest(string CompanyKey);

public partial class Program;
