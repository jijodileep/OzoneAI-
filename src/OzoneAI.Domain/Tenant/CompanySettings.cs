namespace OzoneAI.Domain.Tenant;

public class CompanySettings
{
    public Guid Id { get; set; }

    public string TaxType { get; set; } = "GST";

    public string CurrencyCode { get; set; } = "INR";

    public string CurrencySymbol { get; set; } = "₹";

    public string? StateCode { get; set; }

    public bool MultiGodown { get; set; }

    public bool BatchEnabled { get; set; }

    public string FeatureFlagsJson { get; set; } = "{}";

    public DateTimeOffset UpdatedAt { get; set; }
}
