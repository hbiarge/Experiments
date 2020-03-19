using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Messages;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;
using Shared;

namespace EstimationProcessor.Handlers
{
    public class AllImagesProcessedHandler : IHandleMessages<AllImagesProcessed>
    {
        private readonly IBus _bus;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AllImagesProcessedHandler> _logger;

        public AllImagesProcessedHandler(IBus bus, IHttpClientFactory httpClientFactory, ILogger<AllImagesProcessedHandler> logger)
        {
            _bus = bus;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task Handle(AllImagesProcessed message)
        {
            _logger.LogInformation(
                "Receiving case to estimate with number {caseNumber}.",
                message.CaseNumber);

            var client = _httpClientFactory.CreateClient("estimations");

            var contentString = System.Text.Json.JsonSerializer.Serialize(new
            {
                CallbackUrl = $"{Constants.KnownUris.ApiBaseUri}/ExternalEstimation/{message.CaseNumber:D}",
                Metadata = message.Metadata
            });
            var content = new StringContent(contentString, Encoding.UTF8, MediaTypeNames.Application.Json);

            try
            {
                await client.PostAsync(
                    new Uri($"{Constants.KnownUris.EstimationsBaseUri}/Estimation"),
                    content);

                _logger.LogInformation(
                    "Case {caseNumber} sent to external estimation successfully.",
                    message.CaseNumber);
            }
            catch (Exception e)
            {
                _logger.LogWarning(
                    "Case number {caseNumber} process error. {externalEstimationError}",
                    message.CaseNumber,
                    e.Message);

                await _bus.Send(new UnableToEstimate
                {
                    CaseNumber = message.CaseNumber,
                    Error = e.Message
                });
            }
        }
    }
}
