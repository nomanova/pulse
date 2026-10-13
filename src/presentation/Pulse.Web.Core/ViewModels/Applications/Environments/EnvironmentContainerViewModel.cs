using System.Threading.Tasks;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels.Applications.Environments;

public partial class EnvironmentContainerViewModel : ApplicationContextViewModel
{
    private readonly IFlagManager _flagManager;
    private readonly IPageNavigator _navigator;
    
    public EnvironmentContainerViewModel(
        IContextProvider contextProvider, 
        IFlagManager flagManager, 
        IPageNavigator navigator) : base(contextProvider)
    {
        _flagManager = flagManager;
        _navigator = navigator;
    }

    protected override Task OnApplicationLoadFailed()
    {
        _flagManager.RaiseError("Failed to load environment");
        _navigator.NavigateTo(string.Format(Routes.Applications, OrganizationId), replace: true);

        return Task.CompletedTask;
    }
}