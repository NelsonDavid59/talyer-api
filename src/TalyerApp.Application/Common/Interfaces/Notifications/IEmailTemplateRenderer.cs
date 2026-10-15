namespace TalyerApp.Application.Common.Interfaces.Notifications;

public interface IEmailTemplateRenderer
{
    string Render(string templateName, IReadOnlyDictionary<string, string> placeholders);
}
