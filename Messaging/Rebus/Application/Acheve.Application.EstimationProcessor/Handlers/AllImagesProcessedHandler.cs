using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Acheve.Common.Messages;
using Acheve.Common.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rebus.Bus;
using Rebus.Handlers;

namespace Acheve.Application.EstimationProcessor.Handlers
{
    public class AllImagesProcessedHandler : IHandleMessages<AllImagesProcessed>
    {
        private readonly IBus _bus;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ServicesConfiguration _servicesConfiguration;
        private readonly ILogger<AllImagesProcessedHandler> _logger;

        public AllImagesProcessedHandler(
            IBus bus,
            IHttpClientFactory httpClientFactory,
            IOptions<ServicesConfiguration> servicesConfiguration,
            ILogger<AllImagesProcessedHandler> logger)
        {
            _bus = bus;
            _httpClientFactory = httpClientFactory;
            _servicesConfiguration = servicesConfiguration.Value;
            _logger = logger;
        }

        public async Task Handle(AllImagesProcessed message)
        {
            _logger.LogInformation(
                "New request for external estimation. Case number: {caseNumber}.",
                message.CaseNumber);

            var client = _httpClientFactory.CreateClient("estimations");

            var contentString = System.Text.Json.JsonSerializer.Serialize(new
            {
                CallbackUrl = $"{_servicesConfiguration.Api.BaseUrl}/ExternalEstimation/{message.CaseNumber:D}",
                Metadata = message.Metadata
            });
            var content = new StringContent(contentString, Encoding.UTF8, MediaTypeNames.Application.Json);

            try
            {
                var response = await client.PostAsync(
                    new Uri($"{_servicesConfiguration.Estimations.BaseUrl}/Estimation"),
                    content);

                await ProcessResponse(message, response);
            }
            catch (Exception e)
            {
                _logger.LogWarning(
                    "Error getting estimation for case number {caseNumber}. {externalEstimationError}",
                    message.CaseNumber,
                    e.Message);

                await _bus.Send(new UnableToEstimate
                {
                    CaseNumber = message.CaseNumber,
                    Error = e.Message
                });
            }
        }

        private async Task ProcessResponse(AllImagesProcessed message, HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "Estimation successful for case {caseNumber}.",
                    message.CaseNumber);

                await _bus.Send(new AwaitExternalEstimationToBeProcessed(
                    caseNumber: message.CaseNumber));
            }
            else
            {
                _logger.LogWarning(
                    "Error getting estimation for case number {caseNumber}: StatusCode: {StatusCode}",
                    message.CaseNumber,
                    response.StatusCode);

                await _bus.Send(new UnableToEstimate
                {
                    CaseNumber = message.CaseNumber,
                    Error = $"Response error. Status code: {response.StatusCode}"
                });
            }
        }
    }
}
