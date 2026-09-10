using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Pulse.Web.Core;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Common.Navigation;

public sealed class RedirectNotAuthorized : ComponentBase
{
    [Inject] private NavigationManager NavManager { get; set; } = null!;

    [Inject] private IAuthenticationService AuthService { get; set; } = null!;
    
    protected override async Task OnInitializedAsync()
    {
        await AuthService.SignOut();
        NavManager.NavigateTo(Routes.SignIn);
    }
}