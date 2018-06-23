using System;
using System.Threading.Tasks;
using Messages;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Receiver.Handlers
{
    public class PrintDateTime : IHandleMessages<TimeMessage>
    {
        private readonly ILogger<PrintDateTime> _logger;

        public PrintDateTime(ILogger<PrintDateTime> logger)
        {
            _logger = logger;
        }

        public Task Handle(TimeMessage message)
        {
            _logger.LogInformation("The time is {0}", message.Time);

            return Task.CompletedTask;
        }
    }
}
