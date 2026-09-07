using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Pulse.Web.Core.ViewModels.Common;

public interface IViewModelBase : INotifyPropertyChanged, IDisposable
{
    Task OnInitializedAsync();
}