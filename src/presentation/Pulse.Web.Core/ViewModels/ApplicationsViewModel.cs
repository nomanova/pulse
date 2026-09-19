using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pulse.Api.Ctrl.Client;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels;

public partial class ApplicationsViewModel : ViewModelBase
{
    private const string ApplicationsViewStorageKey = "applications-view";
    private const string ApplicationsViewListValue = "list";
    private const string ApplicationsViewGridValue = "grid";

    private readonly ICtrlApiClient _ctrlApiClient;
    private readonly ILocalStorage _localStorage;

    public ApplicationsViewModel(
        ICtrlApiClient ctrlApiClient,
        ILocalStorage localStorage)
    {
        _ctrlApiClient = ctrlApiClient;
        _localStorage = localStorage;
    }

    [ObservableProperty] public partial string? OrganizationId { get; set; }

    [ObservableProperty] public partial bool IsLoading { get; private set; }

    [ObservableProperty] public partial bool IsListView { get; private set; }

    [ObservableProperty] public partial bool IsGridView { get; private set; }

    [ObservableProperty] public partial string? SearchText { get; set; }

    public override async Task OnInitializedAsync()
    {
        await SetView();
        await LoadApplications();
    }

    partial void OnOrganizationIdChanged(string value)
    {
        // TODO - load applications
    }

    [RelayCommand]
    private async Task OnSearch(string searchText)
    {
        await LoadApplications(searchText);
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

    private async Task LoadApplications(string? searchText = null)
    {
        // TODO
    }
}