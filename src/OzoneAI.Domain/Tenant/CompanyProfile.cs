namespace OzoneAI.Domain.Tenant;

public class CompanyProfile
{
    public Guid Id { get; set; }

    public string LegalName { get; set; } = string.Empty;

    public string? TradeName { get; set; }

    public string Address { get; set; } = string.Empty;

    public string? AddressLocal { get; set; }

    public string Phone { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? TaxNumber { get; set; }

    public string? Fssai { get; set; }

    public string? LogoObjectKey { get; set; }

    public string? BankName { get; set; }

    public string? BankAccountNo { get; set; }

    public string? BankIfsc { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
