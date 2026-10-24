using System.Threading.Tasks;
using Pulse.Web.Common.Components.Base;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Common.Components;

public abstract class CmpFocusOnOpen<TViewModel> : MvvmCmpBase<TViewModel>
    where TViewModel : IViewModelBase
{
    private bool _wasOpen;

    protected abstract bool IsOpen { get; }

    protected abstract Task FocusOnOpenAsync();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        switch (IsOpen)
        {
            case true when !_wasOpen:
                _wasOpen = true;

                await Task.Yield();
                await FocusOnOpenAsync();
                break;
            case false:
                _wasOpen = false;
                break;
        }
    }
}