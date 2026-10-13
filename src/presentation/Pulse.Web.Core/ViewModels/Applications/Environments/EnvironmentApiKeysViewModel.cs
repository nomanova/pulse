using System.Threading.Tasks;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels.Applications.Environments;

public partial class EnvironmentApiKeysViewModel : ViewModelBase
{
    private readonly IContextProvider _contextProvider;

    public EnvironmentApiKeysViewModel(IContextProvider contextProvider)
    {
        _contextProvider = contextProvider;
    }

    protected override string Subtitle => "Api Keys";

    public override Task OnInitializedAsync()
    {
        return Task.CompletedTask;
    }
}