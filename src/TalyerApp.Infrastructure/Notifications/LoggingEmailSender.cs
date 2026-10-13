using Microsoft.Extensions.Logging;
using TalyerApp.Application.Common.Interfaces.Notifications;

namespace TalyerApp.Infrastructure.Notifications;

public class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Email (Development log). To: {To}. Subject: {Subject}. Body: {Body}",
            to,
            subject,
            body);

        return Task.CompletedTask;
    }
}
