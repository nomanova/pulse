using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.Web.Core.Common;
using Pulse.Web.Core.Common.Validation;
using Pulse.Web.Core.Models;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels.Applications.Environments;

public sealed record AddEnvironmentMessage(string ApplicationId);

public sealed record AddEnvironmentModel
{
    public string? Name
    {
        get;
        set
        {
            field = value;
            Key = value.ToKey();
        }
    }

    public string? Key { get; set; }
}

public sealed class AddEnvironmentModelValidator : BaseModelValidator<AddEnvironmentModel>
{
    public AddEnvironmentModelValidator()
    {
        RuleFor(model => model.Name).ValidName();
        RuleFor(model => model.Key).ValidKey();
    }
}

public partial class AddEnvironmentViewModel : ViewModelBase, IRecipient<AddEnvironmentMessage>
{
    private const string ErrAdd = "Could not add environment";
    private const string ErrKeyInUse = "Environment key is in use";

    public readonly AddEnvironmentModelValidator ModelValidator = new();

    private readonly IMessenger _messenger;
    private readonly IContextProvider _contextProvider;
    private readonly ICtrlApiClient _ctrlApiClient;
    private readonly IDialogManager _dialogManager;

    public AddEnvironmentViewModel(
        IMessenger messenger,
        IContextProvider contextProvider,
        ICtrlApiClient ctrlApiClient,
        IDialogManager dialogManager)
    {
        _messenger = messenger;
        _contextProvider = contextProvider;
        _ctrlApiClient = ctrlApiClient;
        _dialogManager = dialogManager;

        messenger.RegisterAll(this);
    }

    public override void Dispose()
    {
        _messenger.UnregisterAll(this);
    }

    [ObservableProperty] public partial bool IsOpen { get; set; }

    [ObservableProperty] public partial AddEnvironmentModel Model { get; private set; } = new();

    [ObservableProperty] public partial bool IsSubmitting { get; set; }

    [ObservableProperty] public partial string? Error { get; set; }

    private string? _applicationId;

    public void Receive(AddEnvironmentMessage message)
    {
        _applicationId = message.ApplicationId;

        Model = new AddEnvironmentModel();
        Error = null;

        IsOpen = true;
    }

    [RelayCommand]
    private async Task OnSubmit()
    {
        IsSubmitting = true;

        var result = await _ctrlApiClient.Environments.Add(new AddEnvironmentRequest
        {
            ApplicationId = _applicationId,
            EnvironmentName = Model.Name,
            EnvironmentKey = Model.Key
        });

        if (!result.IsSuccessWithData())
        {
            IsSubmitting = false;

            Error = result.HasValidationError(Constants.ErrKeyInUse) ? ErrKeyInUse : ErrAdd;
            return;
        }

        var environmentId = result.Data!.Id;

        var fetchResult = await _ctrlApiClient.Environments.Fetch(new FetchEnvironmentRequest
        {
            EnvironmentId = environmentId
        });

        IsSubmitting = false;

        if (!fetchResult.IsSuccessWithData())
        {
            Error = Constants.ErrGeneral;
            return;
        }

        var environment = fetchResult.Data!;
        _contextProvider.AddAndSwitchToEnvironment(_applicationId!, environment.ToIdentity());

        IsOpen = false;
    }

    [RelayCommand]
    private void OnFieldChanged()
    {
        Error = null;
    }

    [RelayCommand]
    private async Task OnKeyHelp()
    {
        await _dialogManager.ShowHelp(Constants.ErrKeyHelp);
    }
}