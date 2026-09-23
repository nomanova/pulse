using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.Web.Core.Helpers;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels.Applications;

public sealed record AddApplicationMessage(string OrganizationId);

public sealed record AddApplicationModel
{
    public string? Name { get; set; }
}

public sealed class AddApplicationModelValidator : BaseModelValidator<AddApplicationModel>
{
    public AddApplicationModelValidator()
    {
        RuleFor(model => model.Name).ValidName();
    }
}

public partial class AddApplicationViewModel : ViewModelBase, IRecipient<AddApplicationMessage>
{
    private const string ErrAdd = "Could not add application";
    private const string ErrNameInUse = "Application name is in use";

    public readonly AddApplicationModelValidator ModelValidator = new();

    private readonly IMessenger _messenger;
    private readonly ICtrlApiClient _ctrlApiClient;
    private readonly IPageNavigator _navigator;

    public AddApplicationViewModel(
        IMessenger messenger,
        ICtrlApiClient ctrlApiClient,
        IPageNavigator navigator)
    {
        _messenger = messenger;
        _ctrlApiClient = ctrlApiClient;
        _navigator = navigator;

        messenger.RegisterAll(this);
    }

    public override void Dispose()
    {
        _messenger.UnregisterAll(this);
    }

    [ObservableProperty] public partial bool IsOpen { get; set; }

    [ObservableProperty] public partial AddApplicationModel Model { get; private set; } = new();

    [ObservableProperty] public partial bool IsSubmitting { get; set; }

    [ObservableProperty] public partial string? Error { get; set; }

    private string? _organizationId;

    public void Receive(AddApplicationMessage message)
    {
        _organizationId = message.OrganizationId;

        Model = new AddApplicationModel();
        Error = null;

        IsOpen = true;
    }

    [RelayCommand]
    private async Task OnSubmit()
    {
        IsSubmitting = true;

        var result = await _ctrlApiClient.Applications.Add(new AddApplicationRequest
        {
            OrganizationId = _organizationId,
            ApplicationName = Model.Name
        });

        IsSubmitting = false;

        if (!result.IsSuccessWithData())
        {
            Error = result.HasValidationError(Constants.ErrNameInUse) ? ErrNameInUse : ErrAdd;
            return;
        }

        var applicationId = result.Data!.Id;
        _navigator.NavigateTo(string.Format(Routes.ApplicationDetail, _organizationId, applicationId));
    }

    [RelayCommand]
    private void OnFieldChanged()
    {
        Error = null;
    }
}