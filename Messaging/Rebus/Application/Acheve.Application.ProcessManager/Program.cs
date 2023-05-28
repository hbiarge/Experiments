using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Acheve.Application.ProcessManager.Handlers;
using Acheve.Common.Messages;
using Acheve.Common.Messages.Tracing;
using Acheve.Common.Shared;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rebus.Config;
using Rebus.Persistence.FileSystem;
using Rebus.Routing.TypeBased;
using Rebus.Sagas.Exclusive;
using Rebus.ServiceProvider;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;

namespace Acheve.Application.ProcessManager
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
                        .WriteTo.ApplicationInsights(
                            Constants.Azure.Apm.ConnectionString, 
                            new TraceTelemetryConverter())
                        .CreateLogger();

                    builder.ClearProviders();
                    builder.AddSerilog();

                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService(config => 
                        config.ConnectionString = Constants.Azure.Apm.ConnectionString);
                    services.AddSingleton<ITelemetryInitializer>(sp => new ServiceNameInitializer(Constants.Services.ProcessManagerService));

                    services.Configure<ServicesConfiguration>(hostContext.Configuration.GetSection("Service"));
                    services.AddSingleton<IPostConfigureOptions<ServicesConfiguration>, ServicesPostConfiguration>();

                    // Automatically register all handlers from the assembly of a given type...
                    services.AutoRegisterHandlersFromAssemblyOf<EstimationSaga>();

                    //Configure Rebus
                    services.AddRebus(
                        configure: configurer => configurer
                            .Options(o =>
                            {
                                o.LogPipeline(verbose: true);
                                o.UseDistributeTracingFlow();
                            })
                            .Logging(l => l.Serilog())
                            .Sagas(s =>
                            {
                                s.EnforceExclusiveAccess();
                                s.UseFilesystem(Constants.FileSystem.SagasPath);
                            })
                            .DataBus(d => d.StoreInBlobStorage(Constants.Azure.Storage.ConnectionString, Constants.Azure.Storage.DataBusContainer))
                            .Transport(t =>
                            {
                                t.UseAzureServiceBus(
                                    Constants.Azure.ServiceBus.ConnectionString, 
                                    Constants.Queues.ProcessManager);
                                t.UseNativeHeaders();
                                t.UseNativeDeadlettering();
                            })
                            .Routing(r => r.TypeBased()
                                .Map<ImageUrlReceived>(Constants.Queues.ImageDownloads)
                                .Map<ImageReady>(Constants.Queues.ImageProcess)
                                .Map<AllImagesProcessed>(Constants.Queues.ExternalEstimations)
                                .Map<EstimationReady>(Constants.Queues.CallbackNotifications)
                                .Map<EstimationError>(Constants.Queues.CallbackNotifications)
                                .Map<EstimationStateChanged>(Constants.Queues.StateHolder)),
                        isDefaultBus: true,
                        onCreated: async bus => { await Task.Delay(0); },
                        startAutomatically: true);

                    services.AddHostedService<Worker>();
                });
    }
}
