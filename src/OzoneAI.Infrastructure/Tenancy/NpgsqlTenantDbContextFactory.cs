using Microsoft.EntityFrameworkCore;
using OzoneAI.Infrastructure.FinancialYears;
using OzoneAI.Infrastructure.Persistence.Tenant;

namespace OzoneAI.Infrastructure.Tenancy;

public sealed class NpgsqlTenantDbContextFactory : ITenantDbContextFactory
{
    public TenantDbContext Create(string connectionString)
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        return new TenantDbContext(options, new FinancialYearContext());
    }
}
