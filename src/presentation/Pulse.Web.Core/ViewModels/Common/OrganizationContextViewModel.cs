using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Pulse.Web.Core.Services;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Core.ViewModels.Common;

public abstract partial class OrganizationContextViewModel : ViewModelBase, IRecipient<OrganizationChangedMessage>
{
    protected readonly IContextProvider ContextProvider;
    protected readonly IMessenger Messenger;

    protected OrganizationContextViewModel(
        IContextProvider contextProvider,
        IMessenger messenger)
    {
        ContextProvider = contextProvider;
        Messenger = messenger;

        Messenger.RegisterAll(this);
    }

    public override void Dispose()
    {
        Messenger.UnregisterAll(this);
    }

    [ObservableProperty] public partial bool IsLoading { get; protected set; }

    [ObservableProperty] public partial string? OrganizationId { get; set; }

    public async Task InitializeOrganizationContext(string organizationId)
    {
        OrganizationId = organizationId;

        IsLoading = true;

        try
        {
            var result = await ContextProvider.EnsureOrganization(organizationId);

            if (result.IsError)
            {
                await OnOrganizationLoadFailed();
                return;
            }

            await OnOrganizationLoaded();
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void Receive(OrganizationChangedMessage message)
    {
        OnOrganizationChanged();
    }

    protected virtual Task OnOrganizationLoaded()
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnOrganizationLoadFailed()
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnOrganizationChanged()
    {
        return Task.CompletedTask;
    }
}