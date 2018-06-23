using System.Threading.Tasks;
using GenericHost.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace GenericHost
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss zzz} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                    theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            var hostBuilder = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<TimerService>();
                    services.AddScoped<IScopedDependency, ScopedDependency>();
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddSerilog();
                });

            await hostBuilder.RunConsoleAsync();
        }
    }
}
