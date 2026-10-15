using TalyerApp.Domain.Shared.Membership;

namespace TalyerApp.Api.Localization;

public sealed class EmailTemplateValidator
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EmailTemplateValidator> _logger;

    public EmailTemplateValidator(
        IWebHostEnvironment environment,
        ILogger<EmailTemplateValidator> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public void Validate()
    {
        var directory = Path.Combine(_environment.ContentRootPath, EmailResourcePaths.Directory);
        var missing = EmailTemplateNames.All
            .Where(name => !File.Exists(Path.Combine(directory, name)))
            .ToList();

        if (missing.Count == 0)
        {
            return;
        }

        var detail = string.Join(", ", missing);
        var message =
            $"Email templates missing in '{directory}': {detail}. Add them under {EmailResourcePaths.Directory}.";
        _logger.LogCritical("{Message}", message);
        throw new InvalidOperationException(message);
    }
}
