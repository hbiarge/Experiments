using System.Threading.Tasks;
using Acheve.Common.Shared;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Acheve.External.Notifications.Features
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(ILogger<NotificationController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Notification([FromBody]NotificationModel model)
        {
            RandomFailureGenerator.RandomFail(
                failThreshold: Constants.FailureThresholds.ExternalNotificationsProcessing, // Exception is thrown if a random value between 0 and 1 is greater or equal this value
                url: Request.GetDisplayUrl(),
                logger: _logger);

            // Simulate some time to process
            await Task.Delay(2000);

            _logger.LogInformation("Received case completion notification. CaseNumber: {caseNumber} with result {result}", model.CaseNumber, model.Result);

            return NoContent();
        }
    }
}
