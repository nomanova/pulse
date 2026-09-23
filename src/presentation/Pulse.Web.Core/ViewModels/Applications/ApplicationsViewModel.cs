using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Dto.Applications;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels.Applications;

public partial class ApplicationsViewModel : ViewModelBase
{
    private const string ErrLoad = "Loading applications failed";

    private const string ApplicationsViewStorageKey = "applications-view";
    private const string ApplicationsViewListValue = "list";
    private const string ApplicationsViewGridValue = "grid";

    private readonly IMessenger _messenger;
    private readonly ICtrlApiClient _ctrlApiClient;
    private readonly ILocalStorage _localStorage;
    private readonly IFlagManager _flagManager;
    private readonly IPageNavigator _navigator;

    public ApplicationsViewModel(
        IMessenger messenger,
        ICtrlApiClient ctrlApiClient,
        ILocalStorage localStorage,
        IFlagManager flagManager, 
        IPageNavigator navigator)
    {
        _messenger = messenger;
        _ctrlApiClient = ctrlApiClient;
        _localStorage = localStorage;
        _flagManager = flagManager;
        _navigator = navigator;
    }

    protected override string Subtitle => "Applications";

    [ObservableProperty] public partial string? OrganizationId { get; set; }

    [ObservableProperty] public partial bool IsLoading { get; private set; }

    [ObservableProperty] public partial bool IsListView { get; private set; }

    [ObservableProperty] public partial bool IsGridView { get; private set; }

    [ObservableProperty] public partial string? SearchText { get; set; }

    [ObservableProperty] public partial bool IsInitialized { get; set; }

    [ObservableProperty] public partial List<ApplicationDto> Applications { get; set; } = [];

    [ObservableProperty] public partial bool HasMoreItems { get; private set; }

    private string? _lastId;

    public override async Task OnInitializedAsync()
    {
        await SetView();
    }

    partial void OnOrganizationIdChanged(string? value)
    {
        Task.Run(() => LoadApplications());
    }

    [RelayCommand]
    private async Task OnLoadMore()
    {
        await LoadApplications(SearchText, _lastId);
    }
    
    [RelayCommand]
    private async Task OnSearch(string searchText)
    {
        await LoadApplications(searchText, _lastId);
    }

    [RelayCommand]
    private async Task OnDisplayGrid(bool toggled = true)
    {
        if (IsGridView)
        {
            return;
        }

        IsListView = !toggled;
        IsGridView = toggled;

        await _localStorage.SetItemAsStringAsync(ApplicationsViewStorageKey, ApplicationsViewGridValue);
    }

    [RelayCommand]
    private async Task OnDisplayList(bool toggled = true)
    {
        if (IsListView)
        {
            return;
        }

        IsListView = toggled;
        IsGridView = !toggled;

        await _localStorage.SetItemAsStringAsync(ApplicationsViewStorageKey, ApplicationsViewListValue);
    }

    [RelayCommand]
    private void OnCreateApplication()
    {
        if (OrganizationId == null)
        {
            return;
        }

        var message = new AddApplicationMessage(OrganizationId);
        _messenger.Send(message);
    }

    [RelayCommand]
    private void OnApplicationDetail(string applicationId)
    {
        _navigator.NavigateTo(string.Format(Routes.ApplicationDetail, OrganizationId, applicationId));
    }

    private async Task SetView()
    {
        var value = await _localStorage.GetItemAsStringAsync(ApplicationsViewStorageKey);

        switch (value)
        {
            case ApplicationsViewListValue:
                await OnDisplayList();
                break;
            default:
                await OnDisplayGrid();
                break;
        }
    }

    private async Task LoadApplications(string? searchText = null, string? lastId = null)
    {
        var query = string.IsNullOrEmpty(searchText) ? null : searchText;

        IsLoading = true;

        var result = await _ctrlApiClient.Applications.Search(new SearchApplicationsRequest
        {
            OrganizationId = OrganizationId,
            Query = query,
            LastId = lastId
        });

        IsLoading = false;

        if (!result.IsSuccessWithData())
        {
            _flagManager.RaiseError(ErrLoad);
            return;
        }

        var response = result.Data!;

        if (string.IsNullOrEmpty(lastId))
        {
            // Initial load
            Applications = [.. response.Entities];
        }
        else
        {
            // Continuation
            Applications.AddRange(response.Entities);
        }

        HasMoreItems = response.HasNext;
        _lastId = response.HasNext ? response.Entities[^1].Id : null;

        OnPropertyChanged(nameof(Applications));

        IsInitialized = true;
    }
}