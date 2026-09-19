namespace Pulse.Web.Core;

public static class Routes
{
    public const string AppHome = "";

    // App
    public const string Applications = "org/{0}/apps";

    // Account
    public const string SignIn = "sign-in";
    public const string NewOrganization = "new-organization";
    public const string SelectOrganization = "select-organization";

    // Common
    public const string Error404 = "404";
    public const string Error503 = "503";
}