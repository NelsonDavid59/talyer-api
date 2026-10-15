using Microsoft.Extensions.Hosting;
using TalyerApp.Application.Common.Interfaces.Notifications;
using TalyerApp.Domain.Shared.Membership;

namespace TalyerApp.Infrastructure.Notifications;

public sealed class FileEmailTemplateRenderer : IEmailTemplateRenderer
{
    private readonly string _templatesDirectory;

    public FileEmailTemplateRenderer(IHostEnvironment environment)
    {
        _templatesDirectory = Path.Combine(environment.ContentRootPath, EmailResourcePaths.Directory);
    }

    public string Render(string templateName, IReadOnlyDictionary<string, string> placeholders)
    {
        var path = Path.Combine(_templatesDirectory, templateName);
        if (!File.Exists(path))
        {
            throw new InvalidOperationException($"Email template '{templateName}' was not found at '{path}'.");
        }

        var html = File.ReadAllText(path);
        foreach (var (placeholder, value) in placeholders)
        {
            html = html.Replace(placeholder, value, StringComparison.Ordinal);
        }

        return html;
    }
}
