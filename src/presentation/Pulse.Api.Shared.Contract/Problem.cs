namespace Pulse.Api.Shared.Contract;

public sealed record Problem
{
    public string? Code { get; init; }

    public string? Description { get; init; }

    public List<ValidationError>? ValidationErrors { get; init; }

    public string? Trace { get; init; }

    public override string ToString()
    {
        var output = $"Problem: {Code}, {Description}";

        if (ValidationErrors != null && ValidationErrors.Count != 0)
        {
            var errors = ValidationErrors.Select(error => error.ToString()).ToList();
            var validationErrors = string.Join(',', errors);

            output += $", {validationErrors}";
        }

        if (Trace != null)
        {
            output += $", {Trace}";
        }

        return output;
    }
}