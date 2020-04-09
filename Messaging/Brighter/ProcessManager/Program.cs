using System;
using System.Diagnostics;
using Messages;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProcessManager.Handlers;
using Rebus.Config;
using Rebus.Persistence.FileSystem;
using Rebus.Routing.TypeBased;
using Rebus.Sagas.Exclusive;
using Rebus.ServiceProvider;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;
using Shared;

namespace ProcessManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = Constants.Services.ProcessManagerService;

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
                        .Enrich.WithProperty("Application", Constants.Services.ProcessManagerService)
                        .WriteTo.Console()
                        .WriteTo.ApplicationInsights(Constants.ApplicationInsightsInstrumentationKey, new TraceTelemetryConverter())
                        .CreateLogger();

                    builder.ClearProviders();
                    builder.AddSerilog();

                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService(Constants.ApplicationInsightsInstrumentationKey);
                    services.AddSingleton<ITelemetryInitializer>(sp => new ServiceNameInitializer(Constants.Services.ProcessManagerService));

                    // Automatically register all handlers from the assembly of a given type...
                    services.AutoRegisterHandlersFromAssemblyOf<EstimationSaga>();

                    //Configure Rebus
                    services.AddRebus(configure => configure
                        .Options(o => o.LogPipeline(verbose: true))
                        .Logging(l => l.Serilog())
                        .Sagas(s =>
                        {
                            s.EnforceExclusiveAccess();
                            s.UseFilesystem(Constants.FileSystem.SagasPath);
                        })
                        .DataBus(d => d.StoreInBlobStorage(Constants.Azure.Storage.ConnectionString, Constants.Azure.Storage.DataBusContainer))
                        .Transport(t => t.UseAzureServiceBus(Constants.Azure.ServiceBus.ConnectionString, Constants.Queues.ProcessManager))
                        .Routing(r => r.TypeBased()
                            .Map<ImageUrlReceived>(Constants.Queues.ImageDownloads)
                            .Map<ImageReady>(Constants.Queues.ImageProcess)
                            .Map<AllImagesProcessed>(Constants.Queues.ExternalEstimations)
                            .Map<EstimationReady>(Constants.Queues.CallbackNotifications)
                            .Map<EstimationError>(Constants.Queues.CallbackNotifications)
                            .Map<EstimationStateChanged>(Constants.Queues.StateHolder)));

                    services.AddHostedService<Worker>();
                });
    }
}
