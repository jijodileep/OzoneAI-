using Microsoft.Extensions.Logging;
using OzoneAI.Application.Platform;

namespace OzoneAI.Infrastructure.Tenancy;

/// <summary>Hangfire job: nightly ActiveUsers30d / TotalUsersCached / LastUsedAt rollup.</summary>
public sealed class TenantMetricsRollupJob(
    ITenantMetricsService metrics,
    ILogger<TenantMetricsRollupJob> logger)
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting tenant metrics rollup");
        var result = await metrics.RefreshAllAsync(cancellationToken);
        logger.LogInformation(
            "Tenant metrics rollup finished: {Updated} updated, {Failed} failed",
            result.TenantsUpdated,
            result.TenantsFailed);
    }
}
