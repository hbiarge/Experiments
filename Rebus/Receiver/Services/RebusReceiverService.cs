using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.ServiceProvider;

namespace Receiver.Services
{
    public sealed class RebusReceiverService : IHostedService
    {
        private readonly IHostEnvironment _env;
        private readonly ILogger<RebusReceiverService> _logger;

        public RebusReceiverService(
            IHostEnvironment env,
            IServiceProvider serviceProvider,
            ILogger<RebusReceiverService> logger)
        {
            _env = env;

            serviceProvider.UseRebus();

            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Rebus receiver service is starting. Env: {environment}", _env.EnvironmentName);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Rebus receiver service  is stopping.");

            return Task.CompletedTask;
        }
    }
}
