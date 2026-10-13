using System.Threading.Tasks;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels.Applications.Environments;

public partial class EnvironmentViewModel : ApplicationContextViewModel
{
    private readonly IContextProvider _contextProvider;
    private readonly IFlagManager _flagManager;
    private readonly IPageNavigator _navigator;

    public EnvironmentViewModel(
        IContextProvider contextProvider,
        IFlagManager flagManager,
        IPageNavigator navigator) : base(contextProvider)
    {
        _contextProvider = contextProvider;
        _flagManager = flagManager;
        _navigator = navigator;
    }

    protected override string Subtitle => "Environment";

    protected override Task OnApplicationLoaded()
    {
        var environment = _contextProvider.CurrentEnvironment;

        if (environment == null)
        {
            return OnApplicationLoadFailed();
        }

        _navigator.NavigateTo(
            string.Format(Routes.EnvironmentConnections, OrganizationId, ApplicationId, environment.Id),
            replace: true);

        return Task.CompletedTask;
    }

    protected override Task OnApplicationLoadFailed()
    {
        _flagManager.RaiseError("Failed to load environment");
        _navigator.NavigateTo(string.Format(Routes.Applications, OrganizationId), replace: true);

        return Task.CompletedTask;
    }
}