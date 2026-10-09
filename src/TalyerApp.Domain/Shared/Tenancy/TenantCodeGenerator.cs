using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TalyerApp.Domain.Shared.Tenancy;

public static class TenantCodeGenerator
{
    public static string FromCompanyName(string companyName)
    {
        var slug = Slugify(companyName);

        if (string.IsNullOrEmpty(slug))
        {
            slug = TenantCodeConstants.Fallback;
        }

        if (string.Equals(slug, TenantCodeConstants.Platform, StringComparison.Ordinal))
        {
            slug = "c-" + TenantCodeConstants.Platform;
        }

        if (slug.Length > TenantCodeConstants.MaxSlugLength)
        {
            slug = slug[..TenantCodeConstants.MaxSlugLength].TrimEnd('-');
        }

        return slug;
    }

    private static string Slugify(string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (ch is >= 'a' and <= 'z' or >= '0' and <= '9')
            {
                builder.Append(ch);
            }
            else
            {
                builder.Append('-');
            }
        }

        var collapsed = Regex.Replace(builder.ToString(), "-{2,}", "-");
        return collapsed.Trim('-');
    }
}
