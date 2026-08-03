namespace Pulse.Api.Ctrl.Contract;

public sealed record SignInRequest
{
    public string? Username { get; init; }

    public string? Password { get; init; }
}