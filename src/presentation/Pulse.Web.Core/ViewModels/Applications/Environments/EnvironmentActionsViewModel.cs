using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels.Applications.Environments;

public partial class EnvironmentActionsViewModel : ViewModelBase
{
    public const string PrimaryTitle = "Cannot remove environment";

    public const string PrimaryMessage =
        "This is the primary environment of the application and cannot be removed.";

    public const string RemoveWarning =
        "All workflows, connections and workflow instances of this environment will be removed as well. " +
        "This cannot be undone.";

    private const string ErrRemove = "Could not remove environment";

    private readonly IMessenger _messenger;
    private readonly IContextProvider _contextProvider;
    private readonly ICtrlApiClient _ctrlApiClient;
    private readonly IDialogManager _dialogManager;
    private readonly IFlagManager _flagManager;

    public EnvironmentActionsViewModel(
        IMessenger messenger,
        IContextProvider contextProvider,
        ICtrlApiClient ctrlApiClient,
        IDialogManager dialogManager,
        IFlagManager flagManager)
    {
        _messenger = messenger;
        _contextProvider = contextProvider;
        _ctrlApiClient = ctrlApiClient;
        _dialogManager = dialogManager;
        _flagManager = flagManager;
    }

    [ObservableProperty] public partial string? ApplicationId { get; set; }

    [ObservableProperty] public partial bool IsRemoving { get; private set; }

    [RelayCommand]
    private void OnAdd()
    {
        if (ApplicationId == null)
        {
            return;
        }

        var message = new AddEnvironmentMessage(ApplicationId);
        _messenger.Send(message);
    }

    [RelayCommand]
    private async Task OnRemove()
    {
        if (IsRemoving)
        {
            return;
        }

        var environment = _contextProvider.CurrentEnvironment;

        if (ApplicationId == null || environment == null)
        {
            return;
        }

        if (environment.IsPrimary)
        {
            await _dialogManager.ShowInfo(PrimaryTitle, PrimaryMessage);
            return;
        }

        var confirmation = await _dialogManager.GetRemoveConfirmation("environment", environment.Name, RemoveWarning);

        if (!confirmation)
        {
            return;
        }

        IsRemoving = true;

        try
        {
            var result = await _ctrlApiClient.Environments.Remove(new RemoveEnvironmentRequest
            {
                EnvironmentId = environment.Id
            });

            if (!result.Success)
            {
                _flagManager.RaiseError(ErrRemove);
                return;
            }

            // Switches to the primary environment, which also triggers navigation
            _contextProvider.RemoveEnvironment(ApplicationId, environment.Id);
        }
        finally
        {
            IsRemoving = false;
        }
    }
}
