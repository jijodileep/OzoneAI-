using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OzoneAI.Application.FinancialYears;
using OzoneAI.Application.Tenancy;
using OzoneAI.Infrastructure.FinancialYears;
using OzoneAI.Infrastructure.Persistence.Catalog;
using OzoneAI.Infrastructure.Persistence.Tenant;
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

        return services;
    }
}
