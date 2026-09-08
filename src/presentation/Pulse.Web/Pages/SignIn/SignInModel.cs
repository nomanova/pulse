namespace Pulse.Web.Pages.SignIn;

public sealed record SignInModel
{
    public string? Username { get; set; }

    public string? Password { get; set; }
}