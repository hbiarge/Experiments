using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Rebus.ServiceProvider;

namespace ImageDownloads
{
    public class Worker : BackgroundService
    {
        public Worker(IServiceProvider serviceProvider)
        {
            serviceProvider.UseRebus();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
