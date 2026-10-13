namespace TalyerApp.Domain.Shared.Tenancy;

public static class TenantTypeExtensions
{
    public static string ToStorageString(this TenantType type) => type switch
    {
        TenantType.Platform => TenantTypeStorage.Platform,
        TenantType.Customer => TenantTypeStorage.Customer,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown tenant type.")
    };

    public static TenantType FromStorageString(string value)
    {
        if (string.Equals(value, TenantTypeStorage.Platform, StringComparison.Ordinal))
        {
            return TenantType.Platform;
        }

        if (string.Equals(value, TenantTypeStorage.Customer, StringComparison.Ordinal))
        {
            return TenantType.Customer;
        }

        throw new ArgumentException($"Unknown tenant type storage value: '{value}'.", nameof(value));
    }
}
