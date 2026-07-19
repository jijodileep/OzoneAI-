using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OzoneAI.Infrastructure.FinancialYears;

namespace OzoneAI.Infrastructure.Persistence.Tenant;

/// <summary>Design-time factory for EF migrations (tenant schema).</summary>
public sealed class TenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql("Host=localhost;Port=5433;Database=ozone_t_demo;Username=ozone;Password=ozone_dev_password")
            .Options;

        return new TenantDbContext(options, new FinancialYearContext());
    }
}
