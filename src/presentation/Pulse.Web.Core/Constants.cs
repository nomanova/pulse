namespace Pulse.Web.Core;

public static class Constants
{
    public const string AppName = "Pulse";
    
    public const string ErrNameInUse = "General.NameInUse";
    public const string ErrKeyInUse = "General.KeyInUse";
    
    public const uint MaxEnvironmentCount = 10;

    public const string ErrKeyHelp = """
                                     The <b>key</b> of an object is used for <b>programmatic access</b> 
                                     and should preferably remain stable.<br/> <br/>
                                     It must be <b>unique within its scope</b> and formatted as 
                                     a <b>valid RFC 1123 label</b> (a-z, 0-9 and '-' characters only).
                                     """;
}