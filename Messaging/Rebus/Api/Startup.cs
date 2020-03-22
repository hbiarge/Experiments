using System;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using Messages;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Shared;
using StateHolder;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Constants.ApplicationInsightsInstrumentationKey);
            services.AddSingleton<ITelemetryInitializer>(sp => new ServiceNameInitializer(Constants.Services.Api));

            services.Configure<ServicesConfiguration>(Configuration.GetSection("Service"));
            services.AddSingleton<IPostConfigureOptions<ServicesConfiguration>, ServicesPostConfiguration>();

            services.AddControllers();

            services.AddGrpcClient<StateHolderService.StateHolderServiceClient>((sp, options) =>
                {
                    var servicesConfiguration = sp.GetService<IOptions<ServicesConfiguration>>();
                    options.Address = new Uri(servicesConfiguration.Value.StateHolder.BaseUrl);
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler();

                    if (Environment.IsDevelopment())
                    {
                        handler.ServerCertificateCustomValidationCallback = (message, certificate, chain, errors) => true;
                    }

                    return handler;
                })
                .AddPolicyHandler(PollyDefaults.RetryPolicyBuilder)
                .AddPolicyHandler(PollyDefaults.TimeoutPolicyBuilder);

            services.AddRebus(configure => configure
                .Options(o => o.LogPipeline(verbose: true))
                .Logging(l => l.Serilog())
                .DataBus(d => d.StoreInBlobStorage(Constants.Azure.Storage.ConnectionString, Constants.Azure.Storage.DataBusContainer))
                .Transport(t => t.UseAzureServiceBusAsOneWayClient(Constants.Azure.ServiceBus.ConnectionString))
                .Routing(r => r.TypeBased()
                    .Map<EstimationRequested>(Constants.Queues.ProcessManager)
                    .Map<ImageProcessed>(Constants.Queues.ProcessManager)
                    .Map<EstimationCompleted>(Constants.Queues.ProcessManager)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ApplicationServices.UseRebus();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
