namespace OzoneAI.Application.Platform;

public interface ITenantMetricsService
{
    /// <summary>
    /// Recomputes ActiveUsers30d / TotalUsersCached (and LastUsedAt from max LastLoginAt)
    /// for each catalog company by querying its tenant DB.
    /// </summary>
    Task<TenantMetricsRefreshResult> RefreshAllAsync(CancellationToken cancellationToken = default);
}

public sealed record TenantMetricsRefreshResult(int TenantsUpdated, int TenantsFailed);
