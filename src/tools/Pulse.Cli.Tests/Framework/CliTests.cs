using Pulse.Api.Ctrl.Client;
using Pulse.Cli.Services;
using Pulse.Cli.Tests.Framework.Mocks;
using Spectre.Console;
using Spectre.Console.Cli.Testing;

namespace Pulse.Cli.Tests.Framework;

public abstract class CliTests
{
    protected readonly CommandAppTester App;
    protected readonly ConfigServiceMock ConfigService;
    protected readonly CtrlApiClientMock CtrlApiClient;

    protected CliTests()
    {
        var console = AnsiConsole.Console;
        ConfigService = new ConfigServiceMock();
        CtrlApiClient = new CtrlApiClientMock();

        var registrar = DependencyInjection.GetRegistrar(console);
        
        registrar.RegisterInstance(typeof(IConfigService), ConfigService);
        registrar.RegisterInstance(typeof(ICtrlApiClient), CtrlApiClient);

        App = new CommandAppTester(registrar);
    }
}