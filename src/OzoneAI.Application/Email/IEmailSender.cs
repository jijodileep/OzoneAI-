namespace OzoneAI.Application.Email;

public interface IEmailSender
{
    bool IsConfigured { get; }

    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}

public sealed record EmailMessage(
    string ToAddress,
    string Subject,
    string PlainTextBody,
    string? ToDisplayName = null);
