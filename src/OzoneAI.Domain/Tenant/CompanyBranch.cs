namespace OzoneAI.Domain.Tenant;

public class CompanyBranch
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? TaxNumber { get; set; }

    public bool IsMain { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
