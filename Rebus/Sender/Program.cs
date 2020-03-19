using System.Threading.Tasks;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Sender.Services;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Sender
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
                .ConfigureLogging(builder =>
                {
                    builder.AddSerilog();
                })
                .ConfigureServices(services =>
                {
                    services.AddRebus((config, scopedServices) =>
                    {
                        config
                            .Logging(l => l.Serilog())
                            .Transport(t => t.UseRabbitMqAsOneWayClient("amqp://guest:guest@rabbit:5672"))
                            .Routing(r => r.TypeBased()
                                .Map<TimeMessage>("timeProcessing"));

                        return config;
                    });

                    services.AddHostedService<RebusPublisherService>();
                });

            await hostBuilder.RunConsoleAsync();
        }
    }
}
