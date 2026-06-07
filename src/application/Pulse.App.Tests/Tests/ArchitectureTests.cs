using Pulse.App.Common.Authorization;
using Pulse.App.Common.Dispatcher;

namespace Pulse.App.Tests.Tests;

public class ArchitectureTests
{
    [Fact]
    public void CommandAndQueryHandler_ShouldHaveAuthorizer()
    {
        var assembly = typeof(IAssemblyReference).Assembly;
        var types = assembly.GetTypes();

        var authorizers = GetAuthorizers(types);

        foreach (var type in types)
        {
            var handlerType = type.GetInterfaces().FirstOrDefault(t =>
                t.IsGenericType &&
                (t.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                 t.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                 t.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

            if (handlerType == null)
            {
                continue;
            }

            var handlerGenericType = handlerType.GenericTypeArguments.First();
            var hasAuthorizer = HasAuthorizerFor(authorizers, handlerGenericType);

            if (!hasAuthorizer)
            {
                Assert.Fail($"{handlerGenericType.Name} is missing an authorizer");
            }
        }
    }

    private static bool HasAuthorizerFor(List<Type> authorizers, Type type)
    {
        return authorizers
            .Select(authorizer => authorizer.GenericTypeArguments.First())
            .Any(authorizerType => authorizerType == type);
    }

    private static List<Type> GetAuthorizers(Type[] types)
    {
        return types
            .Select(type => type.GetInterfaces().FirstOrDefault(t =>
                t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IAuthorizer<>)))
            .OfType<Type>()
            .ToList();
    }
}