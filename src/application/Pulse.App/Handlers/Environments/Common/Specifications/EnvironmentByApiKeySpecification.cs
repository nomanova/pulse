using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Environments;

namespace Pulse.App.Handlers.Environments.Common.Specifications;

public sealed class EnvironmentByApiKeySpecification(string apiKey) : Specification<Environment>
{
    public override Expression<System.Func<Environment, bool>> ToExpression()
    {
        return environment => !environment.IsDeleted && 
                              (environment.ApiKey.Primary == apiKey || environment.ApiKey.Secondary == apiKey);
    }
}