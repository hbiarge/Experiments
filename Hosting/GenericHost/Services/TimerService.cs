using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GenericHost.Services
{
    public sealed class TimerService: IHostedService, IDisposable
    {
        private readonly IHostingEnvironment _env;
        private readonly IServiceProvider _provider;
        private readonly ILogger<TimerService> _logger;
        private Timer _timer;

        public TimerService(
            IHostingEnvironment env,
            IServiceProvider provider,
            ILogger<TimerService> logger)
        {
            _env = env;
            _provider = provider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting. Env: {environment}", _env.EnvironmentName);

            _timer = new Timer(
                callback: DoWork,
                state: null,
                dueTime: TimeSpan.Zero,
                period: TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void DoWork(object state)
        {
            using (var scope = _provider.CreateScope())
            {
                var scopedService = scope.ServiceProvider.GetRequiredService<IScopedDependency>();

                _logger.LogInformation(
                    "DoWork with id: {id}. Now: {now}", 
                    scopedService.GetId(),
                    DateTimeOffset.Now);
            }
        }
    }
}
