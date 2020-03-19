using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Shared;

namespace Api.Features.ExternalImageProcess
{
    [ApiController]
    [Route("[controller]")]
    public class ExternalImageProcessController : ControllerBase
    {
        private readonly IBus _bus;
        private readonly ILogger<ExternalImageProcessController> _logger;

        public ExternalImageProcessController(IBus bus, ILogger<ExternalImageProcessController> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        [HttpPost("{caseNumber}/images/{imageId}")]
        public async Task<IActionResult> ProcessNewEstimation(Guid caseNumber, int imageId, [FromBody]ExternalImageProcessed request)
        {
            _logger.LogInformation("Response received from the external image processor service for case number {caseNumber} and image {imageId}", caseNumber, imageId);

            RandomFailureGenerator.RandomFail(_logger);

            var source = new MemoryStream(new byte[1024]);
            source.Write(Encoding.UTF8.GetBytes(request.Metadata));
            source.Position = 0;
            var attachment = await _bus.Advanced.DataBus.CreateAttachment(source);

            var message = new ImageProcessed
            {
                CaseNumber = caseNumber,
                ImageId = imageId,
                MetadataTicket = attachment.Id
            };

            await _bus.Send(message);

            return Accepted();
        }
    }
}
