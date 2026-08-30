namespace Pulse.Domain.Channels;

public sealed record ParameterValidationError(string ParameterKey, string Message);