using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OzoneAI.Application.Auth;
using OzoneAI.Application.FinancialYears;
using OzoneAI.Application.Platform;
using OzoneAI.Application.Tenancy;
using OzoneAI.Domain.Catalog;
using OzoneAI.Domain.Tenant;
using OzoneAI.Infrastructure.Auth;
using OzoneAI.Infrastructure.FinancialYears;
using OzoneAI.Infrastructure.Persistence.Catalog;
using OzoneAI.Infrastructure.Persistence.Tenant;
using OzoneAI.Infrastructure.Platform;
using OzoneAI.Infrastructure.Tenancy;

namespace OzoneAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var catalogConnection = configuration.GetConnectionString("Catalog")
            ?? throw new InvalidOperationException("Connection string 'Catalog' is not configured.");

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(catalogConnection));

        var tenantConnection = configuration.GetConnectionString("Tenant")
            ?? catalogConnection.Replace("Database=ozone_catalog", "Database=ozone_t_demo", StringComparison.OrdinalIgnoreCase);

        services.AddScoped<IFinancialYearContext, FinancialYearContext>();
        services.AddDbContext<TenantDbContext>(options =>
            options.UseNpgsql(tenantConnection));

        services.AddSingleton<IFinancialYearGuard, FinancialYearGuard>();
        services.AddSingleton<ILedgerBalanceCalculator, LedgerBalanceCalculator>();
        services.AddSingleton<IStockQuantityCalculator, StockQuantityCalculator>();
        services.AddScoped<IFinancialYearSwitchService, FinancialYearSwitchService>();
        services.AddScoped<IYearCloseService, YearCloseService>();
        services.AddScoped<IBalanceSheetService, BalanceSheetService>();
        services.AddScoped<ITenantConnectionFactory, TenantConnectionFactory>();
        services.AddScoped<TenantDataSeeder>();
        services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();
        services.AddScoped<ITenantMetricsService, TenantMetricsService>();

        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher<PlatformUser>, PasswordHasher<PlatformUser>>();
        services.AddSingleton<IPasswordHasher<TenantUser>, PasswordHasher<TenantUser>>();
        services.AddScoped<IPlatformAuthService, PlatformAuthService>();

        var signingKey = configuration["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Jwt:SigningKey is not configured.");
        var issuer = configuration["Jwt:Issuer"] ?? "OzoneAI";
        var audience = configuration["Jwt:Audience"] ?? "OzoneAI";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });
        services.AddAuthorization(options =>
        {
            options.AddPolicy("SuperAdminOnly", policy =>
                policy.RequireRole(JwtTokenService.SuperAdminRole));
        });

        return services;
    }
}
