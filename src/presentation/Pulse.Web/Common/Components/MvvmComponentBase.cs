using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Common.Components;

public abstract class MvvmComponentBase<TViewModel> : ComponentBase, IDisposable where TViewModel : IViewModelBase
{
    [Inject] [NotNull] protected TViewModel ViewModel { get; set; } = default!;

    private bool _disposed;

    protected override void OnInitialized()
    {
        ViewModel.PropertyChanged += OnPropertyChanged;
        base.OnInitialized();
    }

    protected override Task OnInitializedAsync()
    {
        return ViewModel.OnInitializedAsync();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Dispose(true);
        GC.SuppressFinalize(this);
        _disposed = true;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        PropertyChanged(e.PropertyName);
        StateHasChanged();
    }

    protected virtual void PropertyChanged(string? propertyName)
    {
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            ViewModel.PropertyChanged -= OnPropertyChanged;
            ViewModel.Dispose();
        }
    }
}