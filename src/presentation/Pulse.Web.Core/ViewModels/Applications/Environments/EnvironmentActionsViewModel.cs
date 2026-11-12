using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels.Applications.Environments;

public partial class EnvironmentActionsViewModel : ViewModelBase
{
    private readonly IMessenger _messenger;

    public EnvironmentActionsViewModel(IMessenger messenger)
    {
        _messenger = messenger;
    }

    [ObservableProperty] public partial string? ApplicationId { get; set; }

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
}