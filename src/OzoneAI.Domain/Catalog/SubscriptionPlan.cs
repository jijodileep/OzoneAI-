namespace OzoneAI.Domain.Catalog;

public class SubscriptionPlan
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Details { get; set; }

    public int MaxUsers { get; set; } = 5;

    public int MaxGodowns { get; set; } = 1;

    /// <summary>JSON object of module flags, e.g. {"crm":true,"pos":true}.</summary>
    public string ModuleFlagsJson { get; set; } = "{}";

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<Company> Companies { get; set; } = new List<Company>();
}
