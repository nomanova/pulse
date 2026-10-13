using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Core.ViewModels.Common;

public abstract partial class ApplicationContextViewModel : OrganizationContextViewModel
{
    private readonly IContextProvider _contextProvider;

    protected ApplicationContextViewModel(IContextProvider contextProvider)
        : base(contextProvider)
    {
        _contextProvider = contextProvider;
    }
    
    [ObservableProperty]
    public partial string? ApplicationId { get; protected set; }
    
    [ObservableProperty]
    public partial string? EnvironmentId { get; protected set; }
    
    public async Task InitializeApplicationContext(
        string organizationId,
        string applicationId,
        string? environmentId = null)
    {
        ApplicationId = applicationId;
        EnvironmentId = environmentId;

        await InitializeOrganizationContext(organizationId);
    }
    
    protected override async Task OnOrganizationLoaded()
    {
        if (ApplicationId is null)
        {
            return;
        }

        var result = await _contextProvider.EnsureApplication(ApplicationId, EnvironmentId);

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
}