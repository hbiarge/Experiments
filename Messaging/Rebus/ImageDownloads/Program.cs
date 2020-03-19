using System;
using System.Diagnostics;
using ImageDownloads.Handlers;
using Messages;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;
using Shared;

namespace ImageDownloads
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = Constants.Services.ImageDownloadService;

            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, builder) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                        .MinimumLevel.Override("System", LogEventLevel.Information)
                        .Enrich.WithProperty("Application", Constants.Services.ImageDownloadService)
                        .WriteTo.Console()
                        .WriteTo.ApplicationInsights(Constants.ApplicationInsightsInstrumentationKey, new TraceTelemetryConverter())
                        .CreateLogger();

                    builder.ClearProviders();
                    builder.AddSerilog();

                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService(Constants.ApplicationInsightsInstrumentationKey);
                    services.AddSingleton<ITelemetryInitializer>(sp => new ServiceNameInitializer(Constants.Services.ImageDownloadService));

                    services.AddHttpClient("images")
                        .AddPolicyHandler(PollyDefaults.RetryPolicyBuilder)
                        .AddPolicyHandler(PollyDefaults.TimeoutPolicyBuilder);

                    // Automatically register all handlers from the assembly of a given type...
                    services.AutoRegisterHandlersFromAssemblyOf<ImageUrlReceivedHandler>();

                    //Configure Rebus
                    services.AddRebus(configure => configure
                        .Options(o => o.LogPipeline(verbose: true))
                        .Logging(l => l.Serilog())
                        .DataBus(d => d.StoreInBlobStorage(Constants.Azure.Storage.ConnectionString, Constants.Azure.Storage.DataBusContainer))
                        .Transport(t => t.UseAzureServiceBus(Constants.Azure.ServiceBus.ConnectionString, Constants.Queues.ImageDownloads))
                        .Routing(r => r.TypeBased()
                            .Map<ImageDownloaded>(Constants.Queues.ProcessManager)
                            .Map<UnableToDownloadImage>(Constants.Queues.ProcessManager)));

                    services.AddHostedService<Worker>();
                });
    }
}
