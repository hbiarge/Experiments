using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Shared;

namespace Api.Features.ExternalEstimation
{
    [ApiController]
    [Route("[controller]")]
    public class ExternalEstimationController : ControllerBase
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly ILogger<ExternalEstimationController> _logger;

        public ExternalEstimationController(
            IAmACommandProcessor commandProcessor, 
            ILogger<ExternalEstimationController> logger)
        {
            _commandProcessor = commandProcessor;
            _logger = logger;
        }

        [HttpPost("{caseNumber}")]
        public async Task<IActionResult> ExternalEstimationResponse(Guid caseNumber, [FromBody]ExternalEstimationProcessed request)
        {
            _logger.LogInformation("Response received from the external estimations service for case number {caseNumber}", caseNumber);

            RandomFailureGenerator.RandomFail(_logger);

            //var source = new MemoryStream(new byte[1024]);
            //source.Write(Encoding.UTF8.GetBytes(request.Estimation));
            //source.Position = 0;
            //var attachment = await _bus.Advanced.DataBus.CreateAttachment(source);

            var message = new EstimationCompletedEvent
            {
                CaseNumber = caseNumber,
                EstimationTicket = "TODO" //attachment.Id
            };

            await _commandProcessor.PostAsync(message);

            return Accepted();
        }
    }
}
