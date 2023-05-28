using Acheve.Application.StateHolder.Handlers;
using Acheve.Application.StateHolder.Services;
using Acheve.Common.Messages.Tracing;
using Acheve.Common.Shared;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Rebus.Config;
using Rebus.ServiceProvider;

namespace Acheve.Application.StateHolder
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(config =>
                config.ConnectionString = Constants.Azure.Apm.ApplicationInsightsInstrumentationKey); 
            services.AddSingleton<ITelemetryInitializer>(sp => new ServiceNameInitializer(Constants.Services.StateHolder));

            services.Configure<ServicesConfiguration>(Configuration.GetSection("Service"));
            services.AddSingleton<IPostConfigureOptions<ServicesConfiguration>, ServicesPostConfiguration>();

            services.AddSingleton<State>();

            services.AddGrpc();

            // Automatically register all handlers from the assembly of a given type...
            services.AutoRegisterHandlersFromAssemblyOf<EstimationStateChangedHandler>();

            services.AddRebus(configure => configure
                .Options(o =>
                {
                    o.LogPipeline(verbose: true);
                    o.UseDistributeTracingFlow();
                })
                .Logging(l => l.Serilog())
                .Transport(t => t.UseAzureServiceBus(Constants.Azure.ServiceBus.ConnectionString, Constants.Queues.StateHolder)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ApplicationServices.StartRebus();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<StateService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
