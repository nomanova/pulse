using Pulse.Api.Data.Client.Services.Interfaces;

namespace Pulse.Api.Data.Client;

public interface IDataApiClient
{
    IWorkflowsService Workflows { get; }
}