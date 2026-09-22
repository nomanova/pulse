using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.Web.Core.Helpers;
using Pulse.Web.Core.Models;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels;

public sealed record NewOrganizationModel
{
    public string? Name { get; set; }
}

public sealed class NewOrganizationModelValidator : BaseModelValidator<NewOrganizationModel>
{
    public NewOrganizationModelValidator()
    {
        RuleFor(model => model.Name).ValidName();
    }
}

public partial class NewOrganizationViewModel : ActionViewModelBase
{
    private const string ErrAddOrganization = "Could not add organization";

    public readonly NewOrganizationModelValidator ModelValidator = new();

    private readonly IPageNavigator _navigator;
    private readonly ICtrlApiClient _ctrlApiClient;
    private readonly IAuthenticationService _authenticationService;

    public NewOrganizationViewModel(
        IPageNavigator navigator,
        ICtrlApiClient ctrlApiClient,
        IAuthenticationService authenticationService)
    {
        _navigator = navigator;
        _ctrlApiClient = ctrlApiClient;
        _authenticationService = authenticationService;
    }
    
    protected override string Subtitle => "New Organization";
    
    [ObservableProperty] public partial NewOrganizationModel Model { get; private set; } = new();

    [RelayCommand]
    private async Task OnSubmit()
    {
        StartLoading();

        var addResult = await _ctrlApiClient.Organizations.Add(new AddOrganizationRequest
        {
            OrganizationName = Model.Name
        });

        if (!addResult.IsSuccessWithData())
        {
            StopLoading();
            SetFailed(ErrAddOrganization);
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

        var organization = fetchResult.Data!.ToOrganizationProfile();
        await _authenticationService.SwitchOrganization(organization);

        _navigator.NavigateTo(string.Format(Routes.Applications, organization.Id), replace: true);
    }
    
    [RelayCommand]
    private static void OnFieldChanged()
    {
        // NOP
    }
}