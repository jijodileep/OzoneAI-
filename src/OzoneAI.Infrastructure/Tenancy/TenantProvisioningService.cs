using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using OzoneAI.Application.Platform;
using OzoneAI.Domain.Catalog;
using OzoneAI.Infrastructure.FinancialYears;
using OzoneAI.Infrastructure.Persistence.Catalog;
using OzoneAI.Infrastructure.Persistence.Tenant;

namespace OzoneAI.Infrastructure.Tenancy;

public sealed partial class TenantProvisioningService(
    CatalogDbContext catalog,
    IConfiguration configuration,
    TenantDataSeeder seeder,
    ILogger<TenantProvisioningService> logger) : ITenantProvisioningService
{
    private static readonly Regex CompanyKeyRegex = CompanyKeyPattern();

    public async Task<IReadOnlyList<TenantSummaryDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await catalog.Companies.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new TenantSummaryDto(
                x.Id,
                x.Name,
                x.CompanyKey,
                x.DatabaseName,
                x.Status.ToString(),
                x.CreatedAt,
                x.LastUsedAt,
                x.ActiveUsers30d,
                x.TotalUsersCached))
            .ToListAsync(cancellationToken);
    }

    public async Task<CreateTenantResult> CreateAsync(
        CreateTenantRequest request,
        CancellationToken cancellationToken = default)
    {
        var companyKey = NormalizeCompanyKey(request.CompanyKey);
        if (!CompanyKeyRegex.IsMatch(companyKey))
        {
            throw new ArgumentException(
                "companyKey must be 2–32 chars: lowercase letter, then letters/digits/underscore.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.AdminUsername) || string.IsNullOrWhiteSpace(request.AdminPassword))
        {
            throw new ArgumentException("Admin username and password are required.");
        }

        if (request.AdminPassword.Length < 8)
        {
            throw new ArgumentException("Admin password must be at least 8 characters.");
        }

        if (await catalog.Companies.AnyAsync(x => x.CompanyKey == companyKey, cancellationToken))
        {
            throw new InvalidOperationException($"Company key '{companyKey}' already exists.");
        }

        var databaseName = $"ozone_t_{companyKey}";
        if (databaseName.Length > 63)
        {
            throw new ArgumentException("Resulting database name exceeds PostgreSQL limit.");
        }

        var planId = request.PlanId
            ?? await catalog.SubscriptionPlans.AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

        var host = configuration["TenantProvisioning:DefaultHost"] ?? "localhost";
        var port = int.TryParse(configuration["TenantProvisioning:DefaultPort"], out var p) ? p : 5433;
        var dbUser = configuration["TenantProvisioning:DbUsername"] ?? "ozone";
        var dbPassword = configuration["TenantProvisioning:DbPassword"] ?? "ozone_dev_password";

        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            CompanyKey = companyKey,
            DatabaseName = databaseName,
            Status = CompanyStatus.Migrating,
            LegacyMigrationStatus = LegacyMigrationStatus.NotStarted,
            TimeZoneId = string.IsNullOrWhiteSpace(request.TimeZoneId) ? "Asia/Kolkata" : request.TimeZoneId.Trim(),
            PlanId = planId,
            CreatedAt = DateTimeOffset.UtcNow,
            ActiveUsers30d = 0,
            TotalUsersCached = 1,
            SchemaVersion = null
        };

        catalog.Companies.Add(company);
        catalog.TenantDbCredentials.Add(new TenantDbCredential
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Role = TenantDbCredentialRole.Write,
            Host = host,
            Port = port,
            Username = dbUser,
            PasswordProtected = dbPassword,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await catalog.SaveChangesAsync(cancellationToken);

        var dbCreated = false;
        try
        {
            await CreateDatabaseIfMissingAsync(databaseName, cancellationToken);
            dbCreated = true;

            var connectionString =
                $"Host={host};Port={port};Database={databaseName};Username={dbUser};Password={dbPassword}";

            var options = new DbContextOptionsBuilder<TenantDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            await using (var tenantDb = new TenantDbContext(options, new FinancialYearContext()))
            {
                await tenantDb.Database.MigrateAsync(cancellationToken);

                var legalName = string.IsNullOrWhiteSpace(request.LegalName) ? company.Name : request.LegalName.Trim();
                await seeder.SeedDefaultsAsync(
                    tenantDb,
                    new TenantSeedOptions(
                        LegalName: legalName,
                        Address: string.IsNullOrWhiteSpace(request.Address) ? "—" : request.Address.Trim(),
                        Phone: string.IsNullOrWhiteSpace(request.Phone) ? "0000000000" : request.Phone.Trim(),
                        Email: request.Email?.Trim(),
                        TaxType: string.IsNullOrWhiteSpace(request.TaxType) ? "GST" : request.TaxType.Trim(),
                        CurrencyCode: string.IsNullOrWhiteSpace(request.CurrencyCode) ? "INR" : request.CurrencyCode.Trim(),
                        AdminUsername: request.AdminUsername.Trim(),
                        AdminPassword: request.AdminPassword,
                        AdminDisplayName: request.AdminDisplayName),
                    cancellationToken);

                company.SchemaVersion = tenantDb.Database.GetAppliedMigrations().LastOrDefault();
            }

            company.Status = CompanyStatus.Active;
            await catalog.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Provisioned tenant {CompanyKey} database {DatabaseName}",
                companyKey,
                databaseName);

            return new CreateTenantResult(
                company.Id,
                company.CompanyKey,
                company.DatabaseName,
                company.Status.ToString(),
                request.AdminUsername.Trim());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Tenant provisioning failed for {CompanyKey}", companyKey);
            await RollbackAsync(company.Id, databaseName, dbCreated, cancellationToken);
            throw;
        }
    }

    private async Task CreateDatabaseIfMissingAsync(string databaseName, CancellationToken cancellationToken)
    {
        var adminCs = configuration["TenantProvisioning:AdminConnection"]
            ?? configuration.GetConnectionString("Catalog")
            ?? throw new InvalidOperationException("No admin connection for CREATE DATABASE.");

        var builder = new NpgsqlConnectionStringBuilder(adminCs) { Database = "postgres" };
        await using var conn = new NpgsqlConnection(builder.ConnectionString);
        await conn.OpenAsync(cancellationToken);

        await using (var existsCmd = new NpgsqlCommand(
            "SELECT 1 FROM pg_database WHERE datname = @name",
            conn))
        {
            existsCmd.Parameters.AddWithValue("name", databaseName);
            var exists = await existsCmd.ExecuteScalarAsync(cancellationToken);
            if (exists is not null)
            {
                throw new InvalidOperationException($"Database '{databaseName}' already exists.");
            }
        }

        // Database names validated via companyKey regex — safe to interpolate as identifier.
        await using var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\"", conn);
        await createCmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task RollbackAsync(
        Guid companyId,
        string databaseName,
        bool dbCreated,
        CancellationToken cancellationToken)
    {
        try
        {
            var creds = await catalog.TenantDbCredentials.Where(x => x.CompanyId == companyId).ToListAsync(cancellationToken);
            catalog.TenantDbCredentials.RemoveRange(creds);
            var company = await catalog.Companies.FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken);
            if (company is not null)
            {
                catalog.Companies.Remove(company);
            }

            await catalog.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to roll back catalog rows for {CompanyId}", companyId);
        }

        if (!dbCreated)
        {
            return;
        }

        try
        {
            var adminCs = configuration["TenantProvisioning:AdminConnection"]
                ?? configuration.GetConnectionString("Catalog")
                ?? throw new InvalidOperationException("No admin connection for DROP DATABASE.");

            var builder = new NpgsqlConnectionStringBuilder(adminCs) { Database = "postgres" };
            await using var conn = new NpgsqlConnection(builder.ConnectionString);
            await conn.OpenAsync(cancellationToken);
            await using var terminate = new NpgsqlCommand(
                """
                SELECT pg_terminate_backend(pid)
                FROM pg_stat_activity
                WHERE datname = @name AND pid <> pg_backend_pid()
                """,
                conn);
            terminate.Parameters.AddWithValue("name", databaseName);
            await terminate.ExecuteNonQueryAsync(cancellationToken);

            await using var drop = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{databaseName}\"", conn);
            await drop.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to drop database {DatabaseName} during rollback", databaseName);
        }
    }

    private static string NormalizeCompanyKey(string companyKey) =>
        companyKey.Trim().ToLowerInvariant();

    [GeneratedRegex("^[a-z][a-z0-9_]{1,31}$")]
    private static partial Regex CompanyKeyPattern();
}
