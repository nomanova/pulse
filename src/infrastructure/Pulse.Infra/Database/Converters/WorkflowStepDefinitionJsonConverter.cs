using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pulse.Domain.Aggregates.Workflows.Enums;
using Pulse.Domain.Aggregates.Workflows.ValueObjects;

namespace Pulse.Infra.Database.Converters;

public sealed class WorkflowStepDefinitionJsonConverter : JsonConverter<IWorkflowStepDefinition>
{
    public override IWorkflowStepDefinition Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);

        if (!document.RootElement.TryGetProperty(nameof(IWorkflowStepDefinition.Type), out var typeProperty))
        {
            throw new JsonException(
                $"Missing workflow step definition discriminator '{nameof(IWorkflowStepDefinition.Type)}'.");
        }

        var type = typeProperty.Deserialize<WorkflowStepDefinitionType>(options);

        return type switch
        {
            WorkflowStepDefinitionType.Provider =>
                document.RootElement.Deserialize<ProviderWorkflowStepDefinition>(options)
                ?? throw new JsonException($"Could not deserialize workflow step definition of type '{type}'."),

            _ => throw new JsonException($"Unsupported workflow step definition type '{type}'.")
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        IWorkflowStepDefinition value,
        JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}