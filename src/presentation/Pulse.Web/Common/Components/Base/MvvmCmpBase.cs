using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Common.Components.Base;

public abstract class MvvmCmpBase<TViewModel> : ComponentBase, IDisposable where TViewModel : IViewModelBase
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
        Dispose(true);
        GC.SuppressFinalize(this);
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
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            ViewModel.PropertyChanged -= OnPropertyChanged;
            ViewModel.Dispose();
        }

        _disposed = true;
    }
}