namespace OzoneAI.Domain.Catalog;

public enum TenantDbCredentialRole
{
    /// <summary>Primary write database.</summary>
    Write = 0,

    /// <summary>Optional read replica for reports/AI.</summary>
    Read = 1
}
