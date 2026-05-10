using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Pulse.Api;

public static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Constants.ExitError;
        }

        return Constants.ExitOk;
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            .UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration).Enrich.FromLogContext()
                    .WriteTo.Console(theme: ConsoleTheme.None, outputTemplate: Constants.LogTemplate);
            });
}