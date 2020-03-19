using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Config;
using Rebus.ServiceProvider;
using Receiver.Handlers;
using Receiver.Services;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Receiver
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
                    services.AutoRegisterHandlersFromAssemblyOf<PrintDateTime>();

                    services.AddRebus((config, scopedServices) =>
                    {
                        config
                            .Logging(l => l.Serilog())
                            .Transport(t => { t.UseRabbitMq("amqp://guest:guest@rabbit:5672", "timeProcessing"); });

                        return config;
                    });
                    
                    services.AddHostedService<RebusReceiverService>();
                });


            await hostBuilder.RunConsoleAsync();
        }
    }
}
