using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Core.ViewModels.Common;

public abstract partial class OrganizationContextViewModel : ViewModelBase
{
    private readonly IContextProvider _contextProvider;

    protected OrganizationContextViewModel(IContextProvider contextProvider)
    {
        _contextProvider = contextProvider;
    }

    [ObservableProperty] public partial bool IsLoading { get; protected set; }

    [ObservableProperty] public partial string? OrganizationId { get; set; }

    public async Task InitializeOrganizationContext(string organizationId)
    {
        OrganizationId = organizationId;

        IsLoading = true;

        try
        {
            var result = await _contextProvider.EnsureOrganization(organizationId);

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

    protected virtual Task OnOrganizationLoaded()
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnOrganizationLoadFailed()
    {
        return Task.CompletedTask;
    }
}