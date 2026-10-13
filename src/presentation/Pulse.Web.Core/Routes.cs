namespace Pulse.Web.Core;

public static class Routes
{
    public const string AppHome = "";

    // Application
    public const string Applications = "org/{0}/apps";
    public const string ApplicationDetail = "org/{0}/app/{1}";

    // Environment
    public const string EnvironmentDetail = "org/{0}/app/{1}/env/{2}";
    public const string EnvironmentConnections = "org/{0}/app/{1}/env/{2}/connections";
    public const string EnvironmentApiKeys = "org/{0}/app/{1}/env/{2}/api-keys";
    
    // Account
    public const string SignIn = "sign-in";
    public const string NewOrganization = "new-organization";
    public const string SelectOrganization = "select-organization";

    // Common
    public const string Error404 = "404";
    public const string Error503 = "503";
}