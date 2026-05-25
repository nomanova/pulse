namespace Pulse.Api.Mgmt.Contract.Users;

public sealed record SignInRequest
{
    public string? Username { get; set; }

    public string? Password { get; set; }
}