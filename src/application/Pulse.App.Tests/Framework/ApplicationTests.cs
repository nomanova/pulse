using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Security;
using Pulse.App.Common.Security.Interfaces;
using Pulse.App.Common.Services.Interfaces;
using Pulse.App.Tests.Framework.Contexts;
using Pulse.App.Tests.Framework.Mocks.Database;
using Pulse.App.Tests.Framework.Mocks.Security;
using Pulse.App.Tests.Framework.Mocks.Services;
using Pulse.Domain.Aggregates.Users.Services;
using Pulse.Domain.Tests.Shared.Builders;
using Pulse.Domain.Tests.Shared.Fakers;

namespace Pulse.App.Tests.Framework;

public abstract class ApplicationTests
{
    private const string DefaultUserPassword = "User123456";
    
    protected readonly ISender Sender;

    protected Mock<IUnitOfWork> UnitOfWork = null!;
    protected Mock<IDatabaseContext> DatabaseContext = null!;
    protected Mock<IJwtProvider> JwtProvider = null!;
    protected Mock<IDateTimeProvider> DateTimeProvider = null!;

    private readonly IServiceCollection _serviceCollection;
    private IUserPasswordHasher _userPasswordHasher = null!;
    private Mock<IUserClaimProvider> _userClaimProvider = null!;

    protected ApplicationTests()
    {
        _serviceCollection = new ServiceCollection();
        _serviceCollection.AddApplication();

        RegisterServices();

        var serviceProvider = _serviceCollection.BuildServiceProvider();
        Sender = (ISender)serviceProvider.GetRequiredService(typeof(ISender));
    }

    private void RegisterServices()
    {
        UnitOfWork = UnitOfWorkMock.Default();
        _serviceCollection.AddScoped<IUnitOfWork>(_ => UnitOfWork.Object);

        DatabaseContext = DatabaseContextMock.Default();
        _serviceCollection.AddScoped<IDatabaseContext>(_ => DatabaseContext.Object);

        JwtProvider = JwtProviderMock.Default();
        _serviceCollection.AddScoped<IJwtProvider>(_ => JwtProvider.Object);

        DateTimeProvider = DateTimeProviderMock.Default();
        _serviceCollection.AddScoped<IDateTimeProvider>(_ => DateTimeProvider.Object);

        _userPasswordHasher = new FakePasswordHasher();
        _serviceCollection.AddScoped<IUserPasswordHasher>(_ => _userPasswordHasher);

        _serviceCollection.AddScoped<ICachedUserProvider, CachedUserProvider>();

        _userClaimProvider = UserClaimProviderMock.Default();
        _serviceCollection.AddScoped<IUserClaimProvider>(_ => _userClaimProvider.Object);
    }
    
    protected UserContext EnsureUser()
    {
        var context = AddUser();
        _userClaimProvider = UserClaimProviderMock.For(context.User);
        return context;
    }
    
    private UserContext AddUser()
    {
        var user = UserBuilder.New().WithPassword(DefaultUserPassword).Build();
        DatabaseContext.WithUsers(user);

        return new UserContext(DefaultUserPassword, user);
    }
}