using System.Net;
using System.Threading;
using Moq;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract.Users;
using Pulse.Api.Shared.Contract;
using Pulse.App.Dto.Users;
using Pulse.Cli.Commands.User;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.User;

public sealed class UserSignInCommandTests : CliTests
{
    public UserSignInCommandTests()
    {
        App.SetDefaultCommand<UserSignInCommand>();
    }

    [Fact]
    public void Run_NoServerSelected_ShouldFail()
    {
        // Act
        var result = App.Run("john");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("No server selected", result.Output);

        CtrlApiClient.UsersMock.Verify(
            users => users.SignIn(It.IsAny<SignInRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public void Run_UsernameAndPassword_ShouldSignIn()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.UsersMock
            .Setup(users => users.SignIn(
                It.IsAny<SignInRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<AuthDto>.ForSuccess(new AuthDto
            {
                AccessToken = "access-token",
                User = null!
            }, HttpStatusCode.OK));

        App.Console.Input.PushTextWithEnter("password");

        // Act
        var result = App.Run("john");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Equal("access-token", ConfigService.SavedConfig.Servers["default"].AccessToken);
    }

    [Fact]
    public void Run_UsernameAndPassword_ShouldSendSignInRequest()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.UsersMock
            .Setup(users => users.SignIn(
                It.IsAny<SignInRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<AuthDto>.ForSuccess(new AuthDto
            {
                AccessToken = "access-token",
                User = null!
            }, HttpStatusCode.OK));

        App.Console.Input.PushTextWithEnter("password");

        // Act
        var result = App.Run("john");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);

        CtrlApiClient.UsersMock.Verify(
            users => users.SignIn(
                It.Is<SignInRequest>(request =>
                    request.Username == "john" &&
                    request.Password == "password"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void Run_UsernameAndPassword_ShouldPrintUserSignedIn()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.UsersMock
            .Setup(users => users.SignIn(
                It.IsAny<SignInRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<AuthDto>.ForSuccess(new AuthDto
            {
                AccessToken = "access-token",
                User = null!
            }, HttpStatusCode.OK));

        App.Console.Input.PushTextWithEnter("password");

        // Act
        var result = App.Run("john");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("User 'john' signed in", result.Output);
    }

    [Fact]
    public void Run_UsernamePrompt_ShouldUseEnteredUsername()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.UsersMock
            .Setup(users => users.SignIn(
                It.IsAny<SignInRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<AuthDto>.ForSuccess(new AuthDto
            {
                AccessToken = "access-token",
                User = null!
            }, HttpStatusCode.OK));

        App.Console.Input.PushTextWithEnter("john");
        App.Console.Input.PushTextWithEnter("password");

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);

        CtrlApiClient.UsersMock.Verify(
            users => users.SignIn(
                It.Is<SignInRequest>(request =>
                    request.Username == "john" &&
                    request.Password == "password"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void Run_SignInFails_ShouldFail()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.UsersMock
            .Setup(users => users.SignIn(
                It.IsAny<SignInRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<AuthDto>.ForFailure(
                HttpStatusCode.Unauthorized,
                problem: new Problem
                {
                    Code = "Users.InvalidCredentials",
                    Description = "Invalid credentials"
                }));

        App.Console.Input.PushTextWithEnter("password");

        // Act
        var result = App.Run("john");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("Invalid credentials", result.Output);
        Assert.Null(ConfigService.Config.Servers["default"].AccessToken);
    }

    private static Config ServerConfig()
    {
        var config = new Config
        {
            Servers =
            {
                ["default"] = new Models.Server
                {
                    Url = "http://localhost:5000"
                }
            }
        };

        config.SetServer("default");

        return config;
    }
}