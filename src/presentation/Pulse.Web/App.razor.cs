using MudBlazor;
using MudBlazor.Utilities;
using Colors = Pulse.Web.Styles.Colors;

namespace Pulse.Web;

public partial class App
{
    private static readonly MudTheme Theme = new()
    {
        PaletteLight =
        {
            Primary = new MudColor(Colors.Primary),
            Secondary = new MudColor(Colors.Secondary),
            Tertiary = new MudColor(Colors.Blue300),
            Info = new MudColor(Colors.Blue200),
            Success = new MudColor(Colors.Success),
            Warning = new MudColor(Colors.Warning),
            Error = new MudColor(Colors.Error)
        }
    };
}