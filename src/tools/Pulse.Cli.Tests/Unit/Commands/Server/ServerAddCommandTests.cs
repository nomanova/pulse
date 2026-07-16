using Pulse.Cli.Commands.Server;
using Pulse.Cli.Tests.Framework;
using Spectre.Console.Cli.Testing;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Server;

public class ServerAddCommandTests : CliTests
{
    [Fact]
    public void Run_NoArguments()
    {
        // Arrange
        var app = new CommandAppTester();
        app.SetDefaultCommand<ServerAddCommand>();
  
        // Act
        var result = app.Run("add");
  
        // Assert
        Assert.Equal(-1, result.ExitCode);
        //Assert.Contains("Hello", result.Output);
    }
}