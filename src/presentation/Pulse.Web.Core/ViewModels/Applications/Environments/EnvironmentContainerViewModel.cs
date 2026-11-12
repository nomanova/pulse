using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels.Applications.Environments;

public partial class EnvironmentContainerViewModel : ApplicationContextViewModel
{
    private readonly IFlagManager _flagManager;
    private readonly IPageNavigator _navigator;

    public EnvironmentContainerViewModel(
        IContextProvider contextProvider,
        IMessenger messenger,
        IFlagManager flagManager,
        IPageNavigator navigator) : base(contextProvider, messenger)
    {
        _flagManager = flagManager;
        _navigator = navigator;
    }

    [ObservableProperty] public partial string? ApplicationName { get; private set; }

    [ObservableProperty] public partial string? EnvironmentName { get; private set; }

    protected override Task OnApplicationLoaded()
    {
        ApplicationName = ContextProvider.Application?.Name;
        EnvironmentName = ContextProvider.CurrentEnvironment?.Name;

        return base.OnApplicationLoaded();
    }

    protected override Task OnApplicationLoadFailed()
    {
        _flagManager.RaiseError("Failed to load environment");
        _navigator.NavigateTo(string.Format(Routes.Applications, OrganizationId), replace: true);

        return Task.CompletedTask;
    }

    protected override Task OnEnvironmentChanged()
    {
        var currentEnvironment = ContextProvider.CurrentEnvironment;

        if (currentEnvironment == null)
        {
            return Task.CompletedTask;
        }

        var route = string.Format(Routes.EnvironmentDetail, OrganizationId, ApplicationId, currentEnvironment.Id);
        _navigator.NavigateTo(route);

        return Task.CompletedTask;
    }
}