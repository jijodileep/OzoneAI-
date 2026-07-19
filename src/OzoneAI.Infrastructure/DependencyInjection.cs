using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OzoneAI.Infrastructure.Persistence.Catalog;

namespace OzoneAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var catalogConnection = configuration.GetConnectionString("Catalog")
            ?? throw new InvalidOperationException("Connection string 'Catalog' is not configured.");

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(catalogConnection));

        return services;
    }
}
