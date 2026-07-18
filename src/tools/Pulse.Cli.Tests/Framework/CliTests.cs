using Pulse.Api.Ctrl.Client;
using Pulse.Cli.Services;
using Pulse.Cli.Tests.Framework.Mocks;
using Spectre.Console.Cli.Testing;

namespace Pulse.Cli.Tests.Framework;

public abstract class CliTests
{
    protected readonly CommandAppTester App;
    protected readonly ConfigServiceMock ConfigService;
    protected readonly CtrlApiClientMock CtrlApiClient;

    protected CliTests()
    {
        ConfigService = new ConfigServiceMock();
        CtrlApiClient = new CtrlApiClientMock();

        var registrar = DependencyInjection.GetRegistrar();
        
        registrar.RegisterInstance(typeof(IConfigService), ConfigService);
        registrar.RegisterInstance(typeof(ICtrlApiClient), CtrlApiClient);

        App = new CommandAppTester(registrar);
    }
}