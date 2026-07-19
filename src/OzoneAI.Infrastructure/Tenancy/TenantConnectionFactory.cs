using Microsoft.EntityFrameworkCore;
using OzoneAI.Application.Tenancy;
using OzoneAI.Domain.Catalog;
using OzoneAI.Infrastructure.Persistence.Catalog;

namespace OzoneAI.Infrastructure.Tenancy;

public sealed class TenantConnectionFactory(CatalogDbContext catalog) : ITenantConnectionFactory
{
    public async Task<string> GetConnectionStringAsync(
        Guid companyId,
        TenantDbCredentialRole role = TenantDbCredentialRole.Write,
        CancellationToken cancellationToken = default)
    {
        var company = await catalog.Companies.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == companyId, cancellationToken)
            ?? throw new InvalidOperationException("Company not found.");

        var cred = await catalog.TenantDbCredentials.AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.CompanyId == companyId && c.Role == role && c.IsActive,
                cancellationToken);

        if (cred is null && role == TenantDbCredentialRole.Read)
        {
            cred = await catalog.TenantDbCredentials.AsNoTracking()
                .FirstOrDefaultAsync(
                    c => c.CompanyId == companyId && c.Role == TenantDbCredentialRole.Write && c.IsActive,
                    cancellationToken);
        }

        if (cred is null)
        {
            throw new InvalidOperationException(
                $"No active {role} database credentials for company '{company.CompanyKey}'.");
        }

        return $"Host={cred.Host};Port={cred.Port};Database={company.DatabaseName};Username={cred.Username};Password={cred.PasswordProtected}";
    }
}
