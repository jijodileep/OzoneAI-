using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using OzoneAI.Application.Email;

namespace OzoneAI.Infrastructure.Email;

public sealed class SmtpEmailSender(
    IOptions<SmtpOptions> options,
    ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly SmtpOptions _options = options.Value;

    public bool IsConfigured => _options.IsConfigured;

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message.ToAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(message.Subject);

        if (!_options.IsConfigured)
        {
            logger.LogWarning(
                "SMTP not configured; logging email to {To}. Subject: {Subject}. Body: {Body}",
                message.ToAddress,
                message.Subject,
                message.PlainTextBody);
            return;
        }

        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(
            _options.FromDisplayName ?? "OzoneAI",
            _options.FromAddress!));
        mime.To.Add(new MailboxAddress(message.ToDisplayName ?? message.ToAddress, message.ToAddress));
        mime.Subject = message.Subject;
        mime.Body = new TextPart("plain") { Text = message.PlainTextBody };

        if (!string.IsNullOrWhiteSpace(_options.PickupDirectory))
        {
            Directory.CreateDirectory(_options.PickupDirectory);
            var path = Path.Combine(
                _options.PickupDirectory,
                $"{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}.eml");
            await mime.WriteToAsync(path, cancellationToken);
            logger.LogInformation("Wrote email pickup file {Path} for {To}", path, message.ToAddress);
            return;
        }

        using var client = new SmtpClient();
        var secure = _options.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
        await client.ConnectAsync(_options.Host!, _options.Port, secure, cancellationToken);
        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            await client.AuthenticateAsync(_options.Username, _options.Password ?? string.Empty, cancellationToken);
        }

        await client.SendAsync(mime, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
        logger.LogInformation("Sent email to {To} subject {Subject}", message.ToAddress, message.Subject);
    }
}
