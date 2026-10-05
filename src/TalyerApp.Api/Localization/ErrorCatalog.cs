using System.Collections.Frozen;
using System.Reflection;
using System.Text.Json;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Api.Localization;

public sealed class ErrorCatalog
{
    private readonly FrozenDictionary<string, string> _messages;

    public ErrorCatalog(IWebHostEnvironment environment)
    {
        var path = Path.Combine(environment.ContentRootPath, "Resources", "Errors", "en.json");
        if (!File.Exists(path))
        {
            throw new InvalidOperationException($"Error catalog not found at '{path}'.");
        }

        var json = File.ReadAllText(path);
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
            ?? throw new InvalidOperationException($"Error catalog at '{path}' is empty or invalid.");

        _messages = dictionary.ToFrozenDictionary(StringComparer.Ordinal);
    }

    public bool TryGetMessage(string code, out string message) =>
        _messages.TryGetValue(code, out message!);

    public IReadOnlyCollection<string> Codes => _messages.Keys;

    public static IReadOnlyCollection<string> GetDomainErrorCodes()
    {
        var codes = new List<string>();
        var nestedTypes = typeof(DomainErrors).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

        foreach (var nestedType in nestedTypes)
        {
            foreach (var field in nestedType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType != typeof(Error))
                {
                    continue;
                }

                var error = (Error)field.GetValue(null)!;
                if (!string.IsNullOrEmpty(error.Code))
                {
                    codes.Add(error.Code);
                }
            }
        }

        return codes;
    }
}
