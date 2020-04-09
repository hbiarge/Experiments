using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Acheve.Common.Messages;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;

namespace Acheve.Application.CallbackNotifier.Handlers
{
    public class EstimationErrorHandler : IHandleMessages<EstimationError>
    {
        private readonly IBus _bus;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EstimationReadyHandler> _logger;

        public EstimationErrorHandler(IBus bus, IHttpClientFactory httpClientFactory, ILogger<EstimationReadyHandler> logger)
        {
            _bus = bus;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task Handle(EstimationError message)
        {
            _logger.LogInformation(
                "Receiving case number {caseNumber} to notify estimation error.",
                message.CaseNumber);

            var client = _httpClientFactory.CreateClient("notifications");
            var contentString = System.Text.Json.JsonSerializer.Serialize(new
            {
                message.CaseNumber,
                Result = "Failure",
                FailureReason = message.Reason
            });

            try
            {
                await client.PostAsync(
                    message.CallbackUri,
                    new StringContent(contentString, Encoding.UTF8, MediaTypeNames.Application.Json));

                _logger.LogInformation(
                    "Case {caseNumber} notified successfully.",
                    message.CaseNumber);

                await _bus.Send(new NotificationCompleted
                {
                    CaseNumber = message.CaseNumber
                });
            }
            catch (Exception e)
            {
                _logger.LogWarning(
                    "Case number {caseNumber} notification error. {notificationError}",
                    message.CaseNumber,
                    e.Message);

                await _bus.Send(new UnableToNotify
                {
                    CaseNumber = message.CaseNumber
                });
            }
        }
    }
}