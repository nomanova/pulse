using CommunityToolkit.Mvvm.ComponentModel;

namespace Pulse.Web.Core.ViewModels.Common;

public partial class ActionViewModelBase : ViewModelBase
{
    protected const string ErrGeneral = "Something went wrong, please try again later";
    
    [ObservableProperty] public partial bool IsLoading { get; private set; }

    [ObservableProperty] public partial bool IsFailed { get; private set; }

    [ObservableProperty] public partial string? ErrorMessage { get; private set; }
    
    protected void SetFailed(string message)
    {
        ErrorMessage = message;
        IsFailed = true;
    }

    protected void StartLoading()
    {
        IsFailed = false;
        IsLoading = true;
    }

    protected void StopLoading()
    {
        IsLoading = false;
    }
}