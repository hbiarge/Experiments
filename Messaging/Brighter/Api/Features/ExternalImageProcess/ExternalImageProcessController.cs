using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Shared;

namespace Api.Features.ExternalImageProcess
{
    [ApiController]
    [Route("[controller]")]
    public class ExternalImageProcessController : ControllerBase
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly ILogger<ExternalImageProcessController> _logger;

        public ExternalImageProcessController(
            IAmACommandProcessor commandProcessor,
            ILogger<ExternalImageProcessController> logger)
        {
            _commandProcessor = commandProcessor;
            _logger = logger;
        }

        [HttpPost("{caseNumber}/images/{imageId}")]
        public async Task<IActionResult> ProcessNewEstimation(Guid caseNumber, int imageId, [FromBody]ExternalImageProcessed request)
        {
            _logger.LogInformation("Response received from the external image processor service for case number {caseNumber} and image {imageId}", caseNumber, imageId);

            RandomFailureGenerator.RandomFail(_logger);

            //var source = new MemoryStream(new byte[1024]);
            //source.Write(Encoding.UTF8.GetBytes(request.Metadata));
            //source.Position = 0;
            //var attachment = await _bus.Advanced.DataBus.CreateAttachment(source);

            var message = new ImageProcessedEvent
            {
                CaseNumber = caseNumber,
                ImageId = imageId,
                MetadataTicket = "TODO" //attachment.Id
            };

            await _commandProcessor.PostAsync(message);

            return Accepted();
        }
    }
}
