using Microsoft.Extensions.Options;
using TalyerApp.Infrastructure.Notifications;

namespace TalyerApp.Api.Configuration;

public sealed class SmtpOptionsValidator : IValidateOptions<SmtpOptions>
{
    public ValidateOptionsResult Validate(string? name, SmtpOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Host))
        {
            failures.Add($"{SmtpOptions.SectionName}:Host is required.");
        }

        if (options.Port <= 0)
        {
            failures.Add($"{SmtpOptions.SectionName}:Port must be a positive number.");
        }

        if (string.IsNullOrWhiteSpace(options.User))
        {
            failures.Add($"{SmtpOptions.SectionName}:User is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Password))
        {
            failures.Add(
                $"{SmtpOptions.SectionName}:Password is required. " +
                "Set a non-empty value in usersecrets.json (Development) or Smtp__Password.");
        }

        if (string.IsNullOrWhiteSpace(options.From))
        {
            failures.Add($"{SmtpOptions.SectionName}:From is required.");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
