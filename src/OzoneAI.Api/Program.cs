using System.Security.Claims;
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
    await SeedTenantAsync(tenant);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

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
}

static async Task SeedTenantAsync(TenantDbContext tenant)
{
    if (!await tenant.CompanyProfiles.AnyAsync())
    {
        tenant.CompanyProfiles.Add(new CompanyProfile
        {
            Id = Guid.NewGuid(),
            LegalName = "Demo Company",
            Address = "Demo Address",
            Phone = "0000000000",
            UpdatedAt = DateTimeOffset.UtcNow
        });
        tenant.CompanySettings.Add(new CompanySettings
        {
            Id = Guid.NewGuid(),
            UpdatedAt = DateTimeOffset.UtcNow
        });
        tenant.CompanyBranches.Add(new CompanyBranch
        {
            Id = Guid.NewGuid(),
            Name = "Main",
            Address = "Demo Address",
            IsMain = true,
            UpdatedAt = DateTimeOffset.UtcNow
        });
    }

    if (!await tenant.FinancialYears.AnyAsync())
    {
        var start = new DateOnly(DateTime.UtcNow.Year, 4, 1);
        if (DateOnly.FromDateTime(DateTime.UtcNow) < start)
        {
            start = start.AddYears(-1);
        }

        tenant.FinancialYears.Add(new FinancialYear
        {
            Id = Guid.NewGuid(),
            Name = $"{start.Year}-{(start.Year + 1).ToString()[^2..]}",
            StartDate = start,
            EndDate = start.AddYears(1).AddDays(-1),
            Status = FinancialYearStatus.Open,
            IsDefault = true,
            CreatedAt = DateTimeOffset.UtcNow
        });
    }

    await tenant.SaveChangesAsync();
}

internal sealed record SwitchFyRequest(Guid FinancialYearId);

internal sealed record PlatformLoginRequest(string Username, string Password);

public partial class Program;
