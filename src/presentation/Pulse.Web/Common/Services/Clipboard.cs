using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Common.Services;

public class Clipboard : IClipboard
{
    private readonly ILogger<Clipboard> _logger;
    private readonly IJSRuntime _jsRuntime;
    private readonly IFlagManager _flagManager;

    public Clipboard(
        ILogger<Clipboard> logger,
        IJSRuntime jsRuntime,
        IFlagManager flagManager)
    {
        _logger = logger;
        _jsRuntime = jsRuntime;
        _flagManager = flagManager;
    }

    public async Task<bool> CopyTo(string value, bool withMessage = false)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", value);

            if (withMessage)
            {
                _flagManager.RaiseSuccess("Value copied to clipboard");
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Clipboard error: {Message}", ex.Message);

            if (withMessage)
            {
                _flagManager.RaiseError("Could not copy value to clipboard");
            }

            return false;
        }
    }
}