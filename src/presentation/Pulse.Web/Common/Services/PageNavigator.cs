using Microsoft.AspNetCore.Components;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Common.Services;

public sealed class PageNavigator : IPageNavigator
{
    private readonly NavigationManager _navigationManager;
    
    public PageNavigator(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public void NavigateTo(string route, bool forceLoad = false, bool replace = false)
    {
        _navigationManager.NavigateTo(route, forceLoad, replace);
    }
}