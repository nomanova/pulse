namespace Pulse.Api.Shared.Contract;

public sealed record ValidationError(string? Code, string? Description)
{
    public override string ToString()
    {
        return $"ValidationError: {Code}, {Description}";
    }
}