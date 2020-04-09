using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;

namespace Notifications.Features
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
        public IActionResult Notification([FromBody]NotificationModel model)
        {
            _logger.LogInformation("Received case completion notification. CaseNumber: {caseNumber} with result {result}", model.CaseNumber, model.Result);

            RandomFailureGenerator.RandomFail(_logger);

            return NoContent();
        }
    }
}
