using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Notifications;

public interface IEmailSender
{
    Task<Result> SendAsync(
        string to,
        string subject,
        string body,
        bool isHtml,
        CancellationToken cancellationToken = default);
}
