using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pulse.Infra.Database.Converters;

internal static class JsonSerializerOptionsProvider
{
    internal static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}

public sealed class JsonValueConverter<T> : ValueConverter<T, string>
{
    public JsonValueConverter()
        : base(
            value => JsonSerializer.Serialize(value, JsonSerializerOptionsProvider.Options),
            json => JsonSerializer.Deserialize<T>(json, JsonSerializerOptionsProvider.Options)!)
    {
    }
}