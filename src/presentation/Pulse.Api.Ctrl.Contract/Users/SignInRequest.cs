namespace Pulse.Api.Ctrl.Contract.Users;

public sealed record SignInRequest
{
    public string? Username { get; set; }

    public string? Password { get; set; }
}