using System;
using System.Reflection;
using Jaeger;
using Jaeger.Samplers;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JaegerServiceCollectionExtensions
    {
        public static IServiceCollection AddJaeger(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<ITracer>(serviceProvider =>
            {
                string serviceName = Assembly.GetEntryAssembly().GetName().Name;

                ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                ISampler sampler = new ConstSampler(sample: true);

                ITracer tracer = new Tracer.Builder(serviceName)
                    .WithLoggerFactory(loggerFactory)
                    .WithSampler(sampler)
                    .Build();

                GlobalTracer.Register(tracer);

                return tracer;
            });

            return services;
        }
    }
}
