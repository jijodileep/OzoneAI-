using Microsoft.EntityFrameworkCore;
using OzoneAI.Application.FinancialYears;
using OzoneAI.Domain.Tenant;
using OzoneAI.Domain.Tenant.Transactions;

namespace OzoneAI.Infrastructure.Persistence.Tenant;

public sealed class TenantDbContext : DbContext
{
    private readonly IFinancialYearContext _financialYearContext;

    public TenantDbContext(
        DbContextOptions<TenantDbContext> options,
        IFinancialYearContext financialYearContext)
        : base(options)
    {
        _financialYearContext = financialYearContext;
    }

    public DbSet<CompanyProfile> CompanyProfiles => Set<CompanyProfile>();
    public DbSet<CompanyBranch> CompanyBranches => Set<CompanyBranch>();
    public DbSet<CompanySettings> CompanySettings => Set<CompanySettings>();
    public DbSet<FinancialYear> FinancialYears => Set<FinancialYear>();
    public DbSet<LedgerOpeningBalance> LedgerOpeningBalances => Set<LedgerOpeningBalance>();
    public DbSet<StockOpeningBalance> StockOpeningBalances => Set<StockOpeningBalance>();
    public DbSet<SampleSaleDocument> SampleSales => Set<SampleSaleDocument>();

    public DbSet<TenantUser> Users => Set<TenantUser>();

    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TenantDbContext).Assembly,
            t => t.Namespace == typeof(TenantDbContext).Namespace);

        modelBuilder.Entity<SampleSaleDocument>().HasQueryFilter(s =>
            _financialYearContext.FinancialYearId == null
            || s.FinancialYearId == _financialYearContext.FinancialYearId);

        base.OnModelCreating(modelBuilder);
    }
}
