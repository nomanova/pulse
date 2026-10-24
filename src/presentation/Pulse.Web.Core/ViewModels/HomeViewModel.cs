using System.Linq;
using System.Threading.Tasks;
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

    private readonly IContextProvider _contextProvider;
    private readonly IPageNavigator _navigator;
    private readonly IFlagManager _flagManager;
    private readonly IAuthenticationService _authenticationService;
    private readonly ICtrlApiClient _ctrlApiClient;

    public HomeViewModel(
        IContextProvider contextProvider,
        IPageNavigator navigator,
        IFlagManager flagManager,
        IAuthenticationService authenticationService,
        ICtrlApiClient ctrlApiClient)
    {
        _contextProvider = contextProvider;
        _navigator = navigator;
        _flagManager = flagManager;
        _authenticationService = authenticationService;
        _ctrlApiClient = ctrlApiClient;
    }
    
    protected override string Subtitle => "Home";
    
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

        if (availableOrganizations.Count == 1)
        {
            // Select only organization by default
            var defaultOrganization = availableOrganizations[0];
            
            _contextProvider.Organization = new ObjectIdentity(
                defaultOrganization.Id, defaultOrganization.Name, defaultOrganization.Key);
            _navigator.NavigateTo(string.Format(Routes.Applications, defaultOrganization.Id), replace: true);

            return;
        }

        // Set organization
        var currentOrganization = _contextProvider.Organization;

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

        _contextProvider.Organization = new ObjectIdentity(organization.Id, organization.Name, organization.Key);
        _navigator.NavigateTo(string.Format(Routes.Applications, organization.Id), replace: true);
    }
}