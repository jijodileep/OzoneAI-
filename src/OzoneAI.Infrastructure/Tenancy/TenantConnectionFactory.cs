using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OzoneAI.Application.Tenancy;
using OzoneAI.Domain.Catalog;
using OzoneAI.Infrastructure.Persistence.Catalog;

namespace OzoneAI.Infrastructure.Tenancy;

public sealed class TenantConnectionFactory(
    CatalogDbContext catalog,
    IMemoryCache cache) : ITenantConnectionFactory
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    public Task<string> GetWriteConnectionStringAsync(
        Guid companyId,
        CancellationToken cancellationToken = default) =>
        GetConnectionStringAsync(companyId, TenantDbCredentialRole.Write, cancellationToken);

    public Task<string> GetReadConnectionStringAsync(
        Guid companyId,
        CancellationToken cancellationToken = default) =>
        GetConnectionStringAsync(companyId, TenantDbCredentialRole.Read, cancellationToken);

    public async Task<string> GetConnectionStringAsync(
        Guid companyId,
        TenantDbCredentialRole role = TenantDbCredentialRole.Write,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKey(companyId, role);
        if (cache.TryGetValue(cacheKey, out string? cached) && !string.IsNullOrEmpty(cached))
        {
            return cached;
        }

        var info = await ResolveAsync(companyId, role, cancellationToken);
        cache.Set(cacheKey, info.ConnectionString, CacheTtl);

        // If Read fell back to Write, also warm the Write cache entry.
        if (role == TenantDbCredentialRole.Read && info.UsedReadFallback)
        {
            cache.Set(CacheKey(companyId, TenantDbCredentialRole.Write), info.ConnectionString, CacheTtl);
        }

        return info.ConnectionString;
    }

    public void InvalidateCache(Guid companyId)
    {
        cache.Remove(CacheKey(companyId, TenantDbCredentialRole.Write));
        cache.Remove(CacheKey(companyId, TenantDbCredentialRole.Read));
    }

    private async Task<TenantConnectionInfo> ResolveAsync(
        Guid companyId,
        TenantDbCredentialRole role,
        CancellationToken cancellationToken)
    {
        var company = await catalog.Companies.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == companyId, cancellationToken)
            ?? throw new InvalidOperationException("Company not found.");

        var cred = await catalog.TenantDbCredentials.AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.CompanyId == companyId && c.Role == role && c.IsActive,
                cancellationToken);

        var usedFallback = false;
        if (cred is null && role == TenantDbCredentialRole.Read)
        {
            cred = await catalog.TenantDbCredentials.AsNoTracking()
                .FirstOrDefaultAsync(
                    c => c.CompanyId == companyId && c.Role == TenantDbCredentialRole.Write && c.IsActive,
                    cancellationToken);
            usedFallback = cred is not null;
        }

        if (cred is null)
        {
            throw new InvalidOperationException(
                $"No active {role} database credentials for company '{company.CompanyKey}'.");
        }

        var cs =
            $"Host={cred.Host};Port={cred.Port};Database={company.DatabaseName};Username={cred.Username};Password={cred.PasswordProtected}";

        return new TenantConnectionInfo(
            company.Id,
            company.CompanyKey,
            company.DatabaseName,
            usedFallback ? TenantDbCredentialRole.Write : role,
            cs,
            usedFallback);
    }

    private static string CacheKey(Guid companyId, TenantDbCredentialRole role) =>
        $"tenant-cs:{companyId:N}:{role}";
}
