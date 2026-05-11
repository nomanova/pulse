namespace Pulse.Api;

internal static class Constants
{
    public const int ExitOk = 0;
    public const int ExitError = 1;

    public const string LogTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}";
}