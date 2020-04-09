using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Acheve.Common.Messages;
using Acheve.Common.Shared;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rebus.Bus;

namespace Acheve.Application.Api.Features.ExternalEstimation
{
    [ApiController]
    [Route("[controller]")]
    public class ExternalEstimationController : ControllerBase
    {
        private readonly IBus _bus;
        private readonly ILogger<ExternalEstimationController> _logger;

        public ExternalEstimationController(IBus bus, ILogger<ExternalEstimationController> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        [HttpPost("{caseNumber}")]
        public async Task<IActionResult> ExternalEstimationResponse(Guid caseNumber, [FromBody]ExternalEstimationProcessed request)
        {
            RandomFailureGenerator.RandomFail(
                failThreshold: Constants.FailureThresholds.ApplicationReceivingExternalEstimations, // Exception is thrown if a random value between 0 and 1 is greater or equal this value
                url: Request.GetDisplayUrl(),
                _logger);

            _logger.LogInformation("Response received from the external estimations service for case number {caseNumber}", caseNumber);

            var source = new MemoryStream(new byte[1024]);
            source.Write(Encoding.UTF8.GetBytes(request.Estimation));
            source.Position = 0;
            var attachment = await _bus.Advanced.DataBus.CreateAttachment(source);

            var message = new EstimationCompleted
            {
                CaseNumber = caseNumber,
                EstimationTicket = attachment.Id
            };

            await _bus.Send(message);

            return Accepted();
        }
    }
}
