using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;

namespace Estimations.Features
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
            _logger.LogInformation("Received new estimation request. CaseNumber: {caseNumber}", model.CaseNumber);
            _logger.LogInformation("Callback: {callback}", model.CallbackUrl);

            RandomFailureGenerator.RandomFail(_logger);

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
