using System;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

namespace Consumer
{
    public class Handler : ICapSubscribe
    {
        private readonly ILogger<Handler> _logger;

        public Handler(ILogger<Handler> logger)
        {
            _logger = logger;
        }

        [CapSubscribe("test.show.time")]
        public void ReceiveMessage(DateTime time)
        {
            _logger.LogInformation("Received message with content: {date}", time);
        }
    }
}