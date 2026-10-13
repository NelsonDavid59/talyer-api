namespace TalyerApp.Application.Common.Interfaces.Notifications;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
