using System.Text.Json;
using System.Text.Json.Serialization;

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