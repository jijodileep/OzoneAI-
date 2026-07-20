using OzoneAI.Application.Email;

namespace OzoneAI.UnitTests.Support;

internal sealed class CapturingEmailSender : IEmailSender
{
    public bool IsConfigured => true;

    public List<EmailMessage> Sent { get; } = [];

    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        Sent.Add(message);
        return Task.CompletedTask;
    }
}
