using System;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Microsoft.Extensions.DependencyInjection;

namespace Pulse.App.Common.Authorization;

public interface IAuthorizationSender
{
    Task<ErrorOr<Success>> Send(
        IAuthorizationRequirement requirement,
        CancellationToken cancellationToken = default);
}

public sealed class AuthorizationSender(IServiceProvider serviceProvider) 
    : IAuthorizationSender
{
    public Task<ErrorOr<Success>> Send(
        IAuthorizationRequirement requirement,
        CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IAuthorizationHandler<>)
            .MakeGenericType(requirement.GetType());

        var handler = serviceProvider.GetRequiredService(handlerType);

        return ((dynamic)handler).Handle((dynamic)requirement, cancellationToken);
    }
}