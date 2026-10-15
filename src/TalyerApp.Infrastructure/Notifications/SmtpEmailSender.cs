using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using TalyerApp.Application.Common.Interfaces.Notifications;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Infrastructure.Notifications;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Result> SendAsync(
        string to,
        string subject,
        string body,
        bool isHtml,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_options.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart(isHtml ? TextFormat.Html : TextFormat.Plain) { Text = body };

            using var client = new SmtpClient();
            var socketOptions = _options.UseSsl
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.None;

            await client.ConnectAsync(_options.Host, _options.Port, socketOptions, cancellationToken);
            await client.AuthenticateAsync(_options.User, _options.Password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP send failed. To: {To}. Subject: {Subject}.", to, subject);
            return Result.Failure(DomainErrors.MembershipRequest.EmailDeliveryFailed);
        }
    }
}
