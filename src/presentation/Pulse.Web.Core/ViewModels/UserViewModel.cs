using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pulse.Web.Core.Services;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels;

public partial class UserViewModel : ViewModelBase, IRecipient<ContextChangedMessage>
{
    private readonly IMessenger _messenger;
    private readonly IContextProvider _contextProvider;
    private readonly IAuthenticationStore _authenticationStore;
    private readonly IAuthenticationService _authenticationService;
    private readonly IPageNavigator _pageNavigator;
    
    public UserViewModel(
        IMessenger messenger, 
        IContextProvider contextProvider, 
        IAuthenticationStore authenticationStore,
        IAuthenticationService authenticationService,
        IPageNavigator pageNavigator)
    {
        _messenger = messenger;
        _contextProvider = contextProvider;
        _authenticationStore = authenticationStore;
        _authenticationService = authenticationService;
        _pageNavigator = pageNavigator;

        messenger.RegisterAll(this);
    }
    
    public override void Dispose()
    {
        _messenger.UnregisterAll(this);
    }

    [ObservableProperty] public partial string? Username { get; private set; }
    
    [ObservableProperty] public partial string? Organization { get; private set; }

    public override async Task OnInitializedAsync()
    {
        var userProfile = await _authenticationStore.GetUser();
        Username = userProfile?.Username ?? string.Empty;
        
        SetOrganizationName();
    }

    public void Receive(ContextChangedMessage message)
    {
        SetOrganizationName();
    }

    [RelayCommand]
    private void OnSwitchOrganization()
    {
        _pageNavigator.NavigateTo(Routes.SelectOrganization);
    }

    [RelayCommand]
    private async Task OnSignOut()
    {
        await _authenticationService.SignOut();
        _pageNavigator.NavigateTo(Routes.SignIn);
    }

    private void SetOrganizationName()
    {
        Organization = _contextProvider.Organization?.Name;
    }
}