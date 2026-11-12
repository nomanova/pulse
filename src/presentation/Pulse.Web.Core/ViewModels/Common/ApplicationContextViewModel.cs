using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Pulse.Web.Core.Services;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Core.ViewModels.Common;

public abstract partial class ApplicationContextViewModel :
    OrganizationContextViewModel, IRecipient<ApplicationChangedMessage>, IRecipient<EnvironmentChangedMessage>
{
    protected ApplicationContextViewModel(IContextProvider contextProvider, IMessenger messenger)
        : base(contextProvider, messenger)
    {
    }

    [ObservableProperty] public partial string? ApplicationId { get; protected set; }

    [ObservableProperty] public partial string? EnvironmentId { get; protected set; }

    public async Task InitializeApplicationContext(
        string organizationId,
        string applicationId,
        string? environmentId = null)
    {
        ApplicationId = applicationId;
        EnvironmentId = environmentId;

        await InitializeOrganizationContext(organizationId);
    }

    public void Receive(ApplicationChangedMessage message)
    {
        OnApplicationChanged();
    }

    public void Receive(EnvironmentChangedMessage message)
    {
        OnEnvironmentChanged();
    }

    protected override async Task OnOrganizationLoaded()
    {
        if (ApplicationId is null)
        {
            return;
        }

        var result = await ContextProvider.EnsureApplication(ApplicationId, EnvironmentId);

        if (result.IsError)
        {
            await OnApplicationLoadFailed();
            return;
        }

        await OnApplicationLoaded();
    }

    protected virtual Task OnApplicationLoaded()
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnApplicationLoadFailed()
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnApplicationChanged()
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnEnvironmentChanged()
    {
        return Task.CompletedTask;
    }
}