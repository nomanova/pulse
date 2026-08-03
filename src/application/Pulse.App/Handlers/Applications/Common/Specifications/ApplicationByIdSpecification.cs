using Pulse.App.Common.Database.Specifications;
using Pulse.Domain.Aggregates.Applications;
using ApplicationId = Pulse.Domain.Aggregates.Applications.ApplicationId;

namespace Pulse.App.Handlers.Applications.Common.Specifications;

public sealed class ApplicationByIdSpecification(ApplicationId id, bool includeDeleted = false) : 
    ByIdSpecification<Application, ApplicationId>(id, includeDeleted);