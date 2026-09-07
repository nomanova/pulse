using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Pulse.Web.Common.Components;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Common.Services;

public sealed class FlagManager : IFlagManager
{
    private const int SuccessVisibleStateDurationInMs = 3000;
    private const int WarningVisibleStateDurationInMs = 5000;
    private const int ErrorVisibleStateDurationInMs = 5000;
    private const int HideTransitionDurationInMs = 300;
    private const int ShowTransitionDurationInMs = 300;

    private readonly ILogger<FlagManager> _logger;
    private readonly ISnackbar _snackbar;

    public FlagManager(ILogger<FlagManager> logger, ISnackbar snackbar)
    {
        _logger = logger;
        _snackbar = snackbar;
    }
    
    public void RaiseSuccess(string content)
    {
        Raise( "Success", content, Severity.Success, SuccessVisibleStateDurationInMs);
    }

    public void RaiseWarning(string content)
    {
        Raise("Warning", content, Severity.Warning, WarningVisibleStateDurationInMs);
    }

    public void RaiseError(string content)
    {
        Raise("Error", content, Severity.Error, ErrorVisibleStateDurationInMs);
    }
    
    private void Raise(string title, string content, Severity severity, int visibleStateDuration)
    {
        _logger.LogInformation(
            "Logging flag: '{Title}', '{Content}' with severity '{Severity}'", title, content, severity);

        var parameters = new Dictionary<string, object>
        {
            { nameof(CmpSnackbarMessage.Title), title },
            { nameof(CmpSnackbarMessage.Content), content }
        };

        _snackbar.Add<CmpSnackbarMessage>(parameters, severity, options =>
        {
            options.ShowCloseIcon = true;
            options.VisibleStateDuration = visibleStateDuration;
            options.HideTransitionDuration = HideTransitionDurationInMs;
            options.ShowTransitionDuration = ShowTransitionDurationInMs;
        });
    }
}