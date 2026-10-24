using System.Threading.Tasks;
using MudBlazor;
using Pulse.Web.Common.Components;
using Pulse.Web.Common.Components.Dialogs;
using Pulse.Web.Core.Models;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Common.Services;

public sealed class DialogManager : IDialogManager
{
    private static readonly DialogOptions DefaultDialogOptions = new()
    {
        CloseButton = true,
        BackdropClick = false,
        CloseOnEscapeKey = true
    };
    
    private readonly IDialogService _dialogService;

    public DialogManager(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }
    
    public async Task<bool> GetRemoveConfirmation(string item, string? value = null, string? message = null)
    {
        var title = $"Remove {item}?";
        
        var messageLine1 = string.IsNullOrEmpty(value)
            ? $"Are you sure you want to remove this {item}?"
            : $"Are you sure you want to remove {item} '<b>{value}</b>'?";

        var fullMessage = string.IsNullOrEmpty(message) ? messageLine1 : $"{messageLine1}<br/><br/>{message}";

        return await GetConfirmation(title, fullMessage, "Remove", DialogStyle.Warning);
    }
    
    public async Task<bool> GetConfirmation(string title, string message, string action,
        DialogStyle style = DialogStyle.Info)
    {
        var parameters = new DialogParameters
        {
            { nameof(CmpConfirmationDialog.BodyText), message },
            { nameof(CmpConfirmationDialog.ActionText), action },
            { nameof(CmpConfirmationDialog.Style), style }
        };

        var dialog = await _dialogService.ShowAsync<CmpConfirmationDialog>(
            title, parameters, DefaultDialogOptions);

        var result = await dialog.Result;

        return !result?.Canceled ?? false;
    }

    public async Task ShowHelp(string message)
    {
        var parameters = new DialogParameters
        {
            { nameof(CmpHelpDialog.BodyText), message }
        };

        var dialog = await _dialogService.ShowAsync<CmpHelpDialog>(
            "Help", parameters, DefaultDialogOptions);

        await dialog.Result;
    }
}