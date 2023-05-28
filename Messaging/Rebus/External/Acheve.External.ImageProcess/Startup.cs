using Acheve.Common.Shared;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Acheve.External.ImageProcess
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
            services.AddSingleton<ITelemetryInitializer>(sp => new ServiceNameInitializer(Constants.Services.External.ImageProcess));

            services.Configure<ServicesConfiguration>(Configuration.GetSection("Service"));
            services.AddSingleton<IPostConfigureOptions<ServicesConfiguration>, ServicesPostConfiguration>();

            services.AddControllers();

            services.AddHttpClient("ImageProcessConfirmation")
                .AddPolicyHandler(PollyDefaults.RetryPolicyBuilder)
                .AddPolicyHandler(PollyDefaults.TimeoutPolicyBuilder);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
