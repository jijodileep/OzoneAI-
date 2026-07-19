using Microsoft.EntityFrameworkCore;
using OzoneAI.Domain.Catalog;

namespace OzoneAI.Infrastructure.Persistence.Catalog;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<Company> Companies => Set<Company>();

    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();

    public DbSet<TenantDbCredential> TenantDbCredentials => Set<TenantDbCredential>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CatalogDbContext).Assembly,
            t => t.Namespace == typeof(CatalogDbContext).Namespace);
        base.OnModelCreating(modelBuilder);
    }
}
