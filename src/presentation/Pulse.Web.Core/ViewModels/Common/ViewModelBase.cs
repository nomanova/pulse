using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Pulse.Web.Core.ViewModels.Common;

public partial class ViewModelBase : ObservableObject, IViewModelBase
{
    [RelayCommand]
    public virtual async Task OnInitializedAsync()
    {
        await Task.CompletedTask.ConfigureAwait(false);
    }

    public virtual void Dispose()
    {
    }
}