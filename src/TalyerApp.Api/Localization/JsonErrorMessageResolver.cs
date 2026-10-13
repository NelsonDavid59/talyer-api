using TalyerApp.Application.Common.Interfaces.Localization;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Api.Localization;

public class JsonErrorMessageResolver : IErrorMessageResolver
{
    private readonly ErrorCatalog _catalog;
    private readonly ILogger<JsonErrorMessageResolver> _logger;
    private readonly IHostEnvironment _environment;

    public JsonErrorMessageResolver(
        ErrorCatalog catalog,
        ILogger<JsonErrorMessageResolver> logger,
        IHostEnvironment environment)
    {
        _catalog = catalog;
        _logger = logger;
        _environment = environment;
    }

    public string ResolveMessage(IDomainError error)
    {
        if (string.IsNullOrEmpty(error.Code))
        {
            return ResolveByCode(ErrorCodes.Common.Unknown);
        }

        if (_catalog.TryGetMessage(error.Code, out var message))
        {
            return message;
        }

        _logger.LogWarning(
            "Missing catalog entry for error code '{ErrorCode}'. Add to Resources/Errors/en.json.",
            error.Code);

        if (_environment.IsDevelopment())
        {
            _logger.LogError(
                "Missing catalog entry for error code '{ErrorCode}' in Development.",
                error.Code);
        }

        return FormatCatalogEntryMissing(error.Code);
    }

    private string ResolveByCode(string code)
    {
        if (_catalog.TryGetMessage(code, out var message))
        {
            return message;
        }

        return "An unexpected error occurred.";
    }

    private string FormatCatalogEntryMissing(string missingCode)
    {
        if (_catalog.TryGetMessage(ErrorCodes.Common.CatalogEntryMissing, out var template))
        {
            return template.Replace("{code}", missingCode, StringComparison.Ordinal);
        }

        return $"No English message is defined for error code '{missingCode}'. Add an entry to Resources/Errors/en.json.";
    }
}
