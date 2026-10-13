using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Api.Localization;

public class ErrorCatalogValidator
{
    private static readonly string[] RequiredMetaKeys =
    [
        ErrorCodes.Common.Unknown,
        ErrorCodes.Common.CatalogEntryMissing
    ];

    private readonly ErrorCatalog _catalog;
    private readonly ILogger<ErrorCatalogValidator> _logger;

    public ErrorCatalogValidator(ErrorCatalog catalog, ILogger<ErrorCatalogValidator> logger)
    {
        _catalog = catalog;
        _logger = logger;
    }

    public void Validate()
    {
        var missingMeta = RequiredMetaKeys.Where(key => !_catalog.TryGetMessage(key, out _)).ToList();
        if (missingMeta.Count > 0)
        {
            var detail = string.Join(", ", missingMeta);
            var message =
                $"Error catalog is missing required keys: {detail}. Add them to Resources/Errors/en.json.";

            _logger.LogCritical("{Message}", message);
            throw new InvalidOperationException(message);
        }

        foreach (var domainCode in ErrorCatalog.GetDomainErrorCodes())
        {
            if (!_catalog.TryGetMessage(domainCode, out _))
            {
                _logger.LogWarning(
                    "Missing catalog entry for '{Code}'. Add to Resources/Errors/en.json.",
                    domainCode);
            }
        }
    }
}
