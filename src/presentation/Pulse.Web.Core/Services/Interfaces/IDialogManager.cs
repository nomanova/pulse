using System.Threading.Tasks;
using Pulse.Web.Core.Models;

namespace Pulse.Web.Core.Services.Interfaces;

public interface IDialogManager
{
    Task<bool> GetRemoveConfirmation(string item, string? value = null, string? message = null);
    
    Task<bool> GetConfirmation(string title, string message, string action,
        DialogStyle style = DialogStyle.Info);

    Task ShowHelp(string message);
}