using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Extensions;

namespace Pulse.App.Handlers.Applications.Common.Specifications;

public sealed class SearchApplicationsSpecification : Specification<Application>
{
    private readonly OrganizationId _organizationId;
    private readonly string? _query;
    
    public SearchApplicationsSpecification(
        OrganizationId organizationId, 
        string? query)
    {
        _organizationId = organizationId;
        _query = query.AsNormalizedQueryable();
    }
    
    public override Expression<Func<Application, bool>> ToExpression()
    {
        Expression<Func<Application, bool>> expr = application => application.OrganizationId == _organizationId &&
                                                            !application.IsDeleted;
        
        return expr.WithNameFilter(_query);
    }
}