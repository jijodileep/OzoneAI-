using Microsoft.Extensions.Configuration;
using Npgsql;

namespace OzoneAI.Infrastructure.Tenancy;

/// <summary>
/// Resolves host/port/user/password for new tenant DBs.
/// Defaults to the catalog Postgres endpoint so Docker (postgres-catalog:5432)
/// and local (localhost:5433) both work without mismatched hosts.
/// </summary>
public static class TenantDbEndpoint
{
    public static (string Host, int Port, string Username, string Password) Resolve(IConfiguration configuration)
    {
        var catalog = configuration.GetConnectionString("Catalog")
            ?? throw new InvalidOperationException("Connection string 'Catalog' is not configured.");

        var builder = new NpgsqlConnectionStringBuilder(catalog);

        var host = configuration["TenantProvisioning:DefaultHost"];
        if (string.IsNullOrWhiteSpace(host))
        {
            host = builder.Host;
        }

        var port = builder.Port > 0 ? (int)builder.Port : 5432;
        if (int.TryParse(configuration["TenantProvisioning:DefaultPort"], out var configuredPort))
        {
            port = configuredPort;
        }

        var username = configuration["TenantProvisioning:DbUsername"];
        if (string.IsNullOrWhiteSpace(username))
        {
            username = builder.Username;
        }

        var password = configuration["TenantProvisioning:DbPassword"];
        if (string.IsNullOrWhiteSpace(password))
        {
            password = builder.Password;
        }

        if (string.IsNullOrWhiteSpace(host)
            || string.IsNullOrWhiteSpace(username)
            || password is null)
        {
            throw new InvalidOperationException(
                "Could not resolve tenant DB endpoint from Catalog / TenantProvisioning settings.");
        }

        return (host, port, username, password);
    }
}
