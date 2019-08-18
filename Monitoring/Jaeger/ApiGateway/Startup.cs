using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Middleware.Multiplexer;
using OpenTracing.Contrib.NetCore.CoreFx;
using Shared;

namespace ApiGateway
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJaeger();

            // Prevent endless loops when OpenTracing is tracking HTTP requests to Jaeger.
            services.Configure<HttpHandlerDiagnosticOptions>(options =>
            {
                options.IgnorePatterns.Add(request => Constants.JaegerUri.IsBaseOf(request.RequestUri));
            });

            services.AddOpenTracing();

            services.AddOcelot();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOcelot().Wait();
        }
    }
}
