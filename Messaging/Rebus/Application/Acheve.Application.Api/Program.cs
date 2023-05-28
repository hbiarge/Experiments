using System;
using System.Diagnostics;
using Acheve.Common.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;

namespace Acheve.Application.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = Constants.Services.Api;

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
                        .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Information)
                        .MinimumLevel.Override("System", LogEventLevel.Information)
                        .Enrich.WithProperty("Application", Constants.Services.Api)
                        .Enrich.FromLogContext()
                        .Enrich.WithSpan()
                        .WriteTo.Console()
                        .WriteTo.ApplicationInsights(
                            Constants.Azure.Apm.ConnectionString, 
                            new TraceTelemetryConverter())
                        .CreateLogger();

                    builder.ClearProviders();
                    builder.AddSerilog();

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
