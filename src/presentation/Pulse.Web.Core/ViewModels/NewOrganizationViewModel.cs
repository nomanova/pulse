using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.Web.Core.Common;
using Pulse.Web.Core.Common.Validation;
using Pulse.Web.Core.Models;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels;

public sealed record NewOrganizationModel
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

public sealed class NewOrganizationModelValidator : BaseModelValidator<NewOrganizationModel>
{
    public NewOrganizationModelValidator()
    {
        RuleFor(model => model.Name).ValidName();
        RuleFor(model => model.Key).ValidKey();
    }
}

public partial class NewOrganizationViewModel : ActionViewModelBase
{
    private const string ErrAddOrganization = "Could not add organization";
    private const string ErrKeyInUse = "Organization key is in use";

    public readonly NewOrganizationModelValidator ModelValidator = new();

    private readonly IContextProvider _contextProvider;
    private readonly IPageNavigator _navigator;
    private readonly ICtrlApiClient _ctrlApiClient;
    private readonly IDialogManager _dialogManager;

    public NewOrganizationViewModel(
        IContextProvider contextProvider,
        IPageNavigator navigator,
        ICtrlApiClient ctrlApiClient, 
        IDialogManager dialogManager)
    {
        _contextProvider = contextProvider;
        _navigator = navigator;
        _ctrlApiClient = ctrlApiClient;
        _dialogManager = dialogManager;
    }

    protected override string Subtitle => "New Organization";

    [ObservableProperty] public partial NewOrganizationModel Model { get; private set; } = new();

    [RelayCommand]
    private async Task OnSubmit()
    {
        StartLoading();

        var addResult = await _ctrlApiClient.Organizations.Add(new AddOrganizationRequest
        {
            OrganizationName = Model.Name,
            OrganizationKey = Model.Key
        });

        if (!addResult.IsSuccessWithData())
        {
            StopLoading();

            var error = addResult.HasValidationError(Constants.ErrKeyInUse) ? ErrKeyInUse : ErrAddOrganization;
            SetFailed(error);
            return;
        }

        var organizationId = addResult.Data!.Id;

        var fetchResult = await _ctrlApiClient.Organizations.Fetch(new FetchOrganizationRequest
        {
            OrganizationId = organizationId
        });

        StopLoading();

        if (!fetchResult.IsSuccessWithData())
        {
            SetFailed(ErrGeneral);
            return;
        }

        var organization = fetchResult.Data!;
        _contextProvider.Organization = new ObjectIdentity(organization.Id, organization.Name, organization.Key);

        _navigator.NavigateTo(string.Format(Routes.Applications, organization.Id), replace: true);
    }

    [RelayCommand]
    private static void OnFieldChanged()
    {
        // NOP
    }
    
    [RelayCommand]
    private async Task OnKeyHelp()
    {
        await _dialogManager.ShowHelp(Constants.ErrKeyHelp);
    }
}