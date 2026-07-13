using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Models.Enums;

namespace Pulse.App.Common.Context;

public interface IContextProvider
{
    public Scope Scope { get; set; }
    
    public Organization Organization { get; set; }
    
    public Application Application { get; set; }
    
    public Environment Environment { get; set; }
}