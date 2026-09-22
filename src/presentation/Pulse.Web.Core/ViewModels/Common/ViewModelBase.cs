using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Pulse.Web.Core.ViewModels.Common;

public abstract partial class ViewModelBase : ObservableObject, IViewModelBase
{
    [RelayCommand]
    public virtual async Task OnInitializedAsync()
    {
        await Task.CompletedTask.ConfigureAwait(false);
    }

    public virtual void Dispose()
    {
    }

    public string Title => Subtitle == null ? Constants.AppName : $"{Subtitle} | {Constants.AppName}";

    protected virtual string? Subtitle => null;
}