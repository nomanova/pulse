using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Pulse.Web.Common.Components.Base;

public class CmpBase : ComponentBase, IDisposable
{
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    
    protected ElementReference Element { get; set; }
    private DotNetObjectReference<CmpBase>? _reference;
    
    protected DotNetObjectReference<CmpBase> Reference
    {
        get
        {
            _reference ??= DotNetObjectReference.Create(this);
            return _reference;
        }
    }
    
    private readonly Debouncer _debouncer = new();
    
    protected void Debounce(Func<Task> action, int milliseconds = 200)
    {
        _debouncer.Debounce(milliseconds, action);
    }
    
    private bool _disposed;

    public virtual void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _reference?.Dispose();
        _reference = null;
        
        GC.SuppressFinalize(this);
    }
}