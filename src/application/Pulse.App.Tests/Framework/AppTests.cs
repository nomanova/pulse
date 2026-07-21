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
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Roles;
using Pulse.Domain.Aggregates.Users;
using Pulse.Domain.Aggregates.Users.Services;
using Pulse.Tests.Shared.Builders;
using Pulse.Tests.Shared.Fakers;

namespace Pulse.App.Tests.Framework;

public abstract class AppTests
{
    private const string DefaultAdminPassword = "Admin123456";

    protected readonly ISender Sender;

    protected Mock<IUnitOfWork> UnitOfWork = null!;
    protected Mock<IDatabaseContext> DatabaseContext = null!;
    protected Mock<IJwtProvider> JwtProvider = null!;
    protected Mock<IDateTimeProvider> DateTimeProvider = null!;

    private readonly IServiceCollection _serviceCollection;
    private IUserPasswordHasher _userPasswordHasher = null!;
    private Mock<IUserClaimProvider> _userClaimProvider = null!;

    protected AppTests()
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

        _serviceCollection.AddScoped<IUserProvider, UserProvider>();

        _userClaimProvider = UserClaimProviderMock.Default();
        _serviceCollection.AddScoped<IUserClaimProvider>(_ => _userClaimProvider.Object);
    }

    protected UserContext EnsureAdmin()
    {
        var user = UserBuilder.New().WithPassword(DefaultAdminPassword).Build();
        DatabaseContext.AddUsers(user);

        var membership = Membership.Create(user, Role.BuiltIn.SrvOwner);
        DatabaseContext.AddMemberships(membership);
        
        var context = new UserContext(DefaultAdminPassword, user);
        _userClaimProvider = UserClaimProviderMock.For(context.User);

        return context;
    }

    protected Organization EnsureOrganization(User owner)
    {
        var organization = OrganizationBuilder.New().Build();
        DatabaseContext.AddOrganizations(organization);
        
        var membership = Membership.Create(owner, Role.BuiltIn.OrgOwner, organization);
        DatabaseContext.AddMemberships(membership);

        return organization;
    }
}