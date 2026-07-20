using OzoneAI.Domain.Catalog;

namespace OzoneAI.Infrastructure.Tenancy;

/// <summary>Composes Npgsql connection strings from discrete credential fields (never stored as a blob).</summary>
public static class TenantConnectionStringBuilder
{
    public static string Build(string host, int port, string databaseName, string username, string password, string? sslMode = null)
    {
        var cs =
            $"Host={host};Port={port};Database={databaseName};Username={username};Password={password}";
        if (!string.IsNullOrWhiteSpace(sslMode))
        {
            cs += $";SSL Mode={sslMode.Trim()}";
        }

        return cs;
    }

    public static string Build(Company company, TenantDbCredential cred) =>
        Build(cred.Host, cred.Port, company.DatabaseName, cred.Username, cred.PasswordProtected, cred.SslMode);
}
