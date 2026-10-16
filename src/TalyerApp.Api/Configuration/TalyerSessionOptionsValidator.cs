using Microsoft.Extensions.Options;

namespace TalyerApp.Api.Configuration;

public sealed class TalyerSessionOptionsValidator : IValidateOptions<TalyerSessionOptions>
{
    public ValidateOptionsResult Validate(string? name, TalyerSessionOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Issuer))
        {
            failures.Add($"{TalyerSessionOptions.SectionName}:Issuer is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            failures.Add($"{TalyerSessionOptions.SectionName}:Audience is required.");
        }

        if (string.IsNullOrWhiteSpace(options.SigningKey))
        {
            failures.Add(
                $"{TalyerSessionOptions.SectionName}:SigningKey is required. " +
                "Set a non-empty value in usersecrets.json (Development) or Talyer__Session__SigningKey.");
        }
        else if (System.Text.Encoding.UTF8.GetByteCount(options.SigningKey) < 32)
        {
            failures.Add($"{TalyerSessionOptions.SectionName}:SigningKey must be at least 32 bytes.");
        }

        if (options.LifetimeMinutes <= 0)
        {
            failures.Add($"{TalyerSessionOptions.SectionName}:LifetimeMinutes must be greater than 0.");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
