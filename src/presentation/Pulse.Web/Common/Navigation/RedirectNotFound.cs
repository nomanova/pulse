using Microsoft.AspNetCore.Components;
using Pulse.Web.Core;

namespace Pulse.Web.Common.Navigation;

public sealed class RedirectNotFound : ComponentBase
{
    [Inject] private NavigationManager NavManager { get; set; } = null!;
    
    protected override void OnInitialized()
    {
        NavManager.NavigateTo(Routes.Error404);
    }
}