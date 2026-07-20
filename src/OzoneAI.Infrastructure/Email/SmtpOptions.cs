namespace OzoneAI.Infrastructure.Email;

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string? Host { get; set; }

    public int Port { get; set; } = 587;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? FromAddress { get; set; }

    public string? FromDisplayName { get; set; } = "OzoneAI";

    public bool UseStartTls { get; set; } = true;

    /// <summary>When set, messages are written as .eml files instead of sending (local dev).</summary>
    public string? PickupDirectory { get; set; }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(PickupDirectory)
        || (!string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(FromAddress));
}
