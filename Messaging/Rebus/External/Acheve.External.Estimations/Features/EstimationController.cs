using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Acheve.Common.Shared;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Acheve.External.Estimations.Features
{
    [ApiController]
    [Route("[controller]")]
    public class EstimationController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EstimationController> _logger;

        public EstimationController(IHttpClientFactory httpClientFactory, ILogger<EstimationController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessEstimation([FromBody]EstimationModel model)
        {
            RandomFailureGenerator.RandomFail(
                failThreshold: Constants.FailureThresholds.ExternalEstimationsProcessing, // Exception is thrown if a random value between 0 and 1 is greater or equal this value
                url: Request.GetDisplayUrl(), 
                logger: _logger);

            _logger.LogInformation("Received new estimation request. CaseNumber: {caseNumber}", model.CaseNumber);
            _logger.LogInformation("Callback: {callback}", model.CallbackUrl);

            // Simulate some time to process
            await Task.Delay(2000);

            var client = _httpClientFactory.CreateClient("EstimationConfirmation");

            var contentString = System.Text.Json.JsonSerializer.Serialize(new
            {
                Estimation = $"This is the estimation for the case {model.CaseNumber}"
            });

            await client.PostAsync(
                new Uri(model.CallbackUrl),
                new StringContent(contentString, Encoding.UTF8, MediaTypeNames.Application.Json));

            return NoContent();
        }
    }
}
