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
    private const string DefaultUserPassword = "User123456";

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

        _serviceCollection.AddScoped<ICachedUserProvider, CachedUserProvider>();

        _userClaimProvider = UserClaimProviderMock.Default();
        _serviceCollection.AddScoped<IUserClaimProvider>(_ => _userClaimProvider.Object);
    }

    protected UserContext EnsureUser()
    {
        var userContext = AddUser();
        _userClaimProvider = UserClaimProviderMock.For(userContext.User);
        return userContext;
    }

    protected OrganizationContext EnsureOwnedOrganization()
    {
        var organization = AddOrganization();
        var userContext = AddUser();
        AddOwnership(userContext.User, organization);

        _userClaimProvider = UserClaimProviderMock.For(userContext.User);
        
        return new OrganizationContext(userContext.Password, userContext.User, organization);
    }

    private UserContext AddUser()
    {
        var user = UserBuilder.New().WithPassword(DefaultUserPassword).Build();
        DatabaseContext.WithUsers(user);

        return new UserContext(DefaultUserPassword, user);
    }

    private Organization AddOrganization()
    {
        var organization = OrganizationBuilder.New().Build();
        DatabaseContext.WithOrganizations(organization);

        return organization;
    }

    private void AddOwnership(User user, Organization organization)
    {
        var membership = Membership.Create(user, Role.BuiltIn.OrgOwner, organization);
        DatabaseContext.WithMemberships(membership);
    }
}