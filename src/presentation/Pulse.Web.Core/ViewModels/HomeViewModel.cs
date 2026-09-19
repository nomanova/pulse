using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Shared.Contract;
using Pulse.Web.Core.Models;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    private const string ErrFetchOrganizations = "Could not fetch organizations";

    private readonly IPageNavigator _navigator;
    private readonly IFlagManager _flagManager;
    private readonly IAuthenticationService _authenticationService;
    private readonly ICtrlApiClient _ctrlApiClient;

    public HomeViewModel(
        IPageNavigator navigator,
        IFlagManager flagManager,
        IAuthenticationService authenticationService,
        ICtrlApiClient ctrlApiClient)
    {
        _navigator = navigator;
        _flagManager = flagManager;
        _authenticationService = authenticationService;
        _ctrlApiClient = ctrlApiClient;
    }

    [ObservableProperty] public partial string Title { get; private set; } = "Home";

    public override async Task OnInitializedAsync()
    {
        // Fetch available organizations
        var result = await _ctrlApiClient.Organizations.Search(new PagedSearchRequest());

        if (!result.IsSuccessWithData())
        {
            _flagManager.RaiseError(ErrFetchOrganizations);
            await _authenticationService.SignOut();

            _navigator.NavigateTo(Routes.SignIn, replace: true);
            return;
        }

        var availableOrganizations = result.Data!.Entities;

        if (availableOrganizations.Count == 0)
        {
            _navigator.NavigateTo(Routes.NewOrganization, replace: true);
            return;
        }

        // Set organization
        var currentOrganization = await _authenticationService.GetOrganization();

        OrganizationProfile? organization = null;

        if (currentOrganization != null)
        {
            var matchingOrganization = availableOrganizations.FirstOrDefault(org => org.Id == currentOrganization.Id);

            if (matchingOrganization != null)
            {
                organization = matchingOrganization.ToOrganizationProfile();
            }
        }

        if (organization == null)
        {
            _navigator.NavigateTo(Routes.SelectOrganization, replace: true);
            return;
        }

        await _authenticationService.SwitchOrganization(organization);
        _navigator.NavigateTo(string.Format(Routes.Applications, organization.Id), replace: true);
    }
}