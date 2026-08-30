using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pulse.Infra.Database.Converters;

internal static class JsonSerializerOptionsProvider
{
    internal static readonly JsonSerializerOptions Options = CreateOptions();

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new WorkflowStepDefinitionJsonConverter());

        return options;
    }
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