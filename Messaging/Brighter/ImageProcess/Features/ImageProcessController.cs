using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;

namespace ImageProcess.Features
{
    [ApiController]
    [Route("[controller]")]
    public class ImageProcessController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ImageProcessController> _logger;

        public ImageProcessController(IHttpClientFactory httpClientFactory, ILogger<ImageProcessController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessImage([FromForm]ImageProcessModel model)
        {
            _logger.LogInformation("Received new image to process. CaseNumber: {caseNumber}, ImageId: {imageId}", model.CaseNumber, model.ImageId);
            _logger.LogInformation("Callback: {callback}", model.CallbackUrl);

            RandomFailureGenerator.RandomFail(_logger);

            var client = _httpClientFactory.CreateClient("ImageProcessConfirmation");

            var contentString = System.Text.Json.JsonSerializer.Serialize(new
            {
                Metadata = $"This is the metadata tor the case number {model.CaseNumber} image {model.ImageId}"
            });

            await client.PostAsync(
                new Uri(model.CallbackUrl),
                new StringContent(contentString, Encoding.UTF8, MediaTypeNames.Application.Json));

            return NoContent();
        }
    }
}
