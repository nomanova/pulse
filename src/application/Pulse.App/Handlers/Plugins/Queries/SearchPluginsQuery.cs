using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Services.Interfaces;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Plugins;
using Pulse.App.Handlers.Plugins.Common;

namespace Pulse.App.Handlers.Plugins.Queries;

public sealed record SearchPluginsQuery : IQuery<ErrorOr<SearchResultDto<PluginDto>>>;

public sealed class SearchPluginsQueryAuthorizer : PermissionAuthorizer<SearchPluginsQuery>;

public sealed class SearchPluginsQueryHandler :
    IQueryHandler<SearchPluginsQuery, ErrorOr<SearchResultDto<PluginDto>>>
{
    private readonly IPluginManager _pluginManager;

    public SearchPluginsQueryHandler(IPluginManager pluginManager)
    {
        _pluginManager = pluginManager;
    }

    public Task<ErrorOr<SearchResultDto<PluginDto>>> Handle(SearchPluginsQuery request,
        CancellationToken cancellationToken)
    {
        var pluginCatalog = _pluginManager.GetCatalog();

        return Task.FromResult<ErrorOr<SearchResultDto<PluginDto>>>(new SearchResultDto<PluginDto>
        {
            Entities = pluginCatalog.Select(x => x.ToDto()).ToList()
        });
    }
}