﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Shared
{
    public static class PollyDefaults
    {
        // #### Policies ####
        // https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory

        // Retry
        public static readonly Func<IServiceProvider, HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> RetryPolicyBuilder = (di, request) =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>() // thrown by Polly's TimeoutPolicy if the inner call times out
                .WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5)
                    },
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        var logger = di.GetService<ILogger>();
                        logger.LogWarning("Delaying for {delay}ms, then making retry {retry}.", timespan.TotalMilliseconds, retryAttempt);
                    });

        // Timeout for an individual try
        public static readonly Func<IServiceProvider, HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> TimeoutPolicyBuilder = (di, request) =>
            Policy.TimeoutAsync<HttpResponseMessage>(
                seconds: 10,
                onTimeoutAsync: (context, timeout, task) =>
                {
                    var logger = di.GetService<ILogger>();
                    logger.LogWarning("The timeout of {delay}ms for this request has expired.", timeout.TotalMilliseconds);
                    return Task.CompletedTask;
                });
    }
}