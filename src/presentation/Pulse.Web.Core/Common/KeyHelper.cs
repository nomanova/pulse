
using System.Globalization;
using System.Text;
using Pulse.Web.Core.Common.Validation;

namespace Pulse.Web.Core.Common;

public static class KeyHelper
{
    public static string? ToKey(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            var lowerCharacter = char.ToLowerInvariant(character);

            if (lowerCharacter is >= 'a' and <= 'z' or >= '0' and <= '9')
            {
                builder.Append(lowerCharacter);
            }
            else if (builder.Length > 0 && builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        var key = builder
            .ToString()
            .Trim('-');

        if (key.Length > Validators.MaxKeyLength)
        {
            key = key[..Validators.MaxKeyLength].TrimEnd('-');
        }

        return key;
    }
}