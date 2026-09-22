using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Shared.Contract;
using Pulse.App.Dto.Organizations;
using Pulse.Web.Core.Models;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels;

public partial class SelectOrganizationViewModel : ViewModelBase
{
    private const string ErrLoadOrganizations = "Could not load organizations";

    private readonly IAuthenticationService _authenticationService;
    private readonly ICtrlApiClient _ctrlApiClient;
    private readonly IFlagManager _flagManager;
    private readonly IPageNavigator _navigator;

    public SelectOrganizationViewModel(
        IAuthenticationService authenticationService,
        ICtrlApiClient ctrlApiClient,
        IFlagManager flagManager,
        IPageNavigator navigator)
    {
        _authenticationService = authenticationService;
        _ctrlApiClient = ctrlApiClient;
        _flagManager = flagManager;
        _navigator = navigator;
    }

    protected override string Subtitle => "Select Organization";
    
    [ObservableProperty] public partial bool IsLoading { get; private set; }

    [ObservableProperty] public partial OrganizationProfile? CurrentOrganization { get; private set; }

    [ObservableProperty] public partial List<OrganizationDto>? Organizations { get; private set; }

    [ObservableProperty] public partial OrganizationDto? SelectedOrganization { get; private set; }

    public override async Task OnInitializedAsync()
    {
        IsLoading = true;

        CurrentOrganization = await _authenticationService.GetOrganization();

        var result = await _ctrlApiClient.Organizations.Search(new PagedSearchRequest());

        if (!result.IsSuccessWithData())
        {
            _flagManager.RaiseError(ErrLoadOrganizations);
            _navigator.NavigateTo(Routes.AppHome, replace: true);
            return;
        }

        Organizations = result.Data!.Entities.ToList();
        IsLoading = false;
    }

    [RelayCommand]
    private void OnSetSelected(OrganizationDto? item)
    {
        SelectedOrganization = item;
    }

    [RelayCommand]
    private async Task OnConfirm()
    {
        if (SelectedOrganization == null)
        {
            return;
        }

        var organization = SelectedOrganization.ToOrganizationProfile();
        await _authenticationService.SwitchOrganization(organization);

        _navigator.NavigateTo(string.Format(Routes.Applications, organization.Id), replace: true);
    }
    
    [RelayCommand]
    private void OnCancel()
    {
        _navigator.NavigateTo(Routes.AppHome, replace: true);
    }
}