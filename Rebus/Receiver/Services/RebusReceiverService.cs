using System;
using System.Threading;
using System.Threading.Tasks;
using Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Bus;

namespace Receiver.Services
{
    public sealed class RebusReceiverService : IHostedService, IDisposable
    {
        private readonly IHostingEnvironment _env;
        private readonly IBus _bus;
        private readonly ILogger<RebusReceiverService> _logger;

        public RebusReceiverService(
            IHostingEnvironment env,
            IBus bus,
            ILogger<RebusReceiverService> logger)
        {
            _env = env;
            _bus = bus;
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

        public void Dispose()
        {
            _bus?.Dispose();
        }
    }
}
