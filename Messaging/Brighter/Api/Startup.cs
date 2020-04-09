using System;
using Messages;
using Messages.Events;
using Messages.Mappers;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.MessagingGateway.AzureServiceBus;
using Paramore.Brighter.ServiceActivator.Extensions.DependencyInjection;
using Paramore.Brighter.ServiceActivator.TestHelpers;
using Polly;
using Polly.Extensions.Http;
using Shared;
using StateHolder;

namespace Api
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
            services.AddApplicationInsightsTelemetry(Constants.ApplicationInsightsInstrumentationKey);
            services.AddSingleton<ITelemetryInitializer>(sp => new ServiceNameInitializer(Constants.Services.Api));

            services.AddControllers();

            services.AddGrpcClient<StateHolderService.StateHolderServiceClient>(
                    o => o.Address = new Uri(Constants.KnownUris.StateHolderUri))
                .AddPolicyHandler(PollyDefaults.RetryPolicyBuilder)
                .AddPolicyHandler(PollyDefaults.TimeoutPolicyBuilder);

            //services.AddServiceActivator(options =>
            //    {
            //        options.BrighterMessaging = new BrighterMessaging(
            //            messageStore: new InMemoryMessageStore(),
            //            producer: new AzureServiceBusMessageProducer(
            //                new AzureServiceBusMessagingGatewayConfiguration
            //                {
            //                    Namespace = new Namespace
            //                    {
            //                        Name = "messages-demo",
            //                        BaseAddress = new Uri("https://acheve-demo.servicebus.windows.net")
            //                    },
            //                    SharedAccessPolicy = new SharedAccessPolicy
            //                    {
            //                        Name = "RootManageSharedAccessKey",
            //                        Key = "QtBc2CPBtsZWNXTCJFiFqdTxWKGVVXOPbmTeIoPxozc="
            //                    }
            //                }));
            //    })
            //    .MapperRegistryFromAssemblies(typeof(EstimationRequestedEventMessageMapper).Assembly);

            services.AddBrighter(options =>
                {
                    options.BrighterMessaging = new BrighterMessaging(
                        messageStore: new InMemoryMessageStore(),
                        producer: new AzureServiceBusMessageProducer(
                            new AzureServiceBusMessagingGatewayConfiguration
                            {
                                Namespace = new Namespace
                                {
                                    Name = "acheve-demo",
                                    BaseAddress = new Uri("https://acheve-demo.servicebus.windows.net")
                                },
                                SharedAccessPolicy = new SharedAccessPolicy
                                {
                                    Name = "RootManageSharedAccessKey",
                                    Key = "QtBc2CPBtsZWNXTCJFiFqdTxWKGVVXOPbmTeIoPxozc="
                                }
                            }));
                })
                //.AsyncHandlers(registerHandlers=>
                //    registerHandlers.RegisterAsync<>())
                .MapperRegistry(registerMappings =>
                {
                    registerMappings.Register<EstimationRequestedEvent, EstimationRequestedEventMessageMapper>();
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.ApplicationServices.UseRebus();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
