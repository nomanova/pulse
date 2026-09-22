using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Pulse.Web.Common.Components.Base;

public abstract class CmpResizable : CmpBase, IAsyncDisposable
{
    private const double ResizeTolerance = 0.1;

    private sealed record Rect
    {
        public double Width { get; set; }

        public double Height { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

        public double Right { get; set; }

        public double Bottom { get; set; }
    }

    protected double? Width { get; private set; }

    protected double? Height { get; private set; }

    private bool _widthAndHeightAreSet;
    private IJSObjectReference? _module;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/CmpResizable.js");

            var rect = await _module.InvokeAsync<Rect?>("createResizable", Element, Reference);

            if (!_widthAndHeightAreSet && rect != null)
            {
                _widthAndHeightAreSet = true;
                Resize(rect.Width, rect.Height);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_module != null)
        {
            await _module.InvokeAsync<Rect>("destroyResizable", Element);

            await _module.DisposeAsync();
            _module = null;
        }
    }

    [JSInvokable]
    public void Resize(double width, double height)
    {
        var stateHasChanged = false;

        if (Width is null || Math.Abs(width - Width.Value) > ResizeTolerance)
        {
            Width = width;
            stateHasChanged = true;
        }

        if (Height is null || Math.Abs(height - Height.Value) > ResizeTolerance)
        {
            Height = height;
            stateHasChanged = true;
        }

        if (stateHasChanged)
        {
            StateHasChanged();
            OnResize();
        }
    }

    protected virtual void OnResize()
    {
    }
}