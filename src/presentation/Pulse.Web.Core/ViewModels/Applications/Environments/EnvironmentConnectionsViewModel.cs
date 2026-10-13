using System.Threading.Tasks;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels.Applications.Environments;

public partial class EnvironmentConnectionsViewModel : ViewModelBase
{
    private readonly IContextProvider _contextProvider;

    public EnvironmentConnectionsViewModel(IContextProvider contextProvider)
    {
        _contextProvider = contextProvider;
    }

    protected override string Subtitle => "Connections";

    public override Task OnInitializedAsync()
    {
        // TODO
        
        return Task.CompletedTask;
    }
}