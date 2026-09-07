namespace Pulse.Web.Core.Services.Interfaces;

public interface IPageNavigator
{
    void NavigateTo(string route, bool forceLoad = false, bool replace = false);
}