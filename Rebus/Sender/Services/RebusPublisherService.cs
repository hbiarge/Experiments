using System;
using System.Threading;
using System.Threading.Tasks;
using Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Bus;

namespace Sender.Services
{
    public sealed class RebusPublisherService : IHostedService, IDisposable
    {
        private readonly IHostEnvironment _env;
        private readonly IBus _bus;
        private readonly ILogger<RebusPublisherService> _logger;

        private Timer _timer;

        public RebusPublisherService(
            IHostEnvironment env,
            IBus bus,
            ILogger<RebusPublisherService> logger)
        {
            _env = env;
            _bus = bus;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Rebus sender service is starting. Env: {environment}", _env.EnvironmentName);

            _timer = new Timer(
                callback: DoWork,
                state: null,
                dueTime: TimeSpan.Zero,
                period: TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Rebus sender service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _bus?.Dispose();
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Sending new message.");
            _bus.Send(new TimeMessage(DateTimeOffset.Now));
            _logger.LogInformation("New message sent.");
        }
    }
}
