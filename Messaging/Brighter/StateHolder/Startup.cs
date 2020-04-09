﻿using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Config;
using Rebus.ServiceProvider;
using Shared;
using StateHolder.Handlers;

namespace StateHolder
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Constants.ApplicationInsightsInstrumentationKey);
            services.AddSingleton<ITelemetryInitializer>(sp => new ServiceNameInitializer(Constants.Services.StateHolder));

            services.AddSingleton<State>();

            services.AddGrpc();

            // Automatically register all handlers from the assembly of a given type...
            services.AutoRegisterHandlersFromAssemblyOf<EstimationStateChangedHandler>();

            services.AddRebus(configure => configure
                .Options(o => o.LogPipeline(verbose: true))
                .Logging(l => l.Serilog())
                .Transport(t => t.UseAzureServiceBus(Constants.Azure.ServiceBus.ConnectionString, Constants.Queues.StateHolder)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ApplicationServices.UseRebus();

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