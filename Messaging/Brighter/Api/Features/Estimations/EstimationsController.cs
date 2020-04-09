using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Messages;
using Messages.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Shared;
using StateHolder;

namespace Api.Features.Estimations
{
    [ApiController]
    [Route("[controller]")]
    public class EstimationsController : ControllerBase
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly StateHolderService.StateHolderServiceClient _stateHolderService;
        private readonly ILogger<EstimationsController> _logger;

        public EstimationsController(
            IAmACommandProcessor commandProcessor,
            StateHolderService.StateHolderServiceClient stateHolderService,
            ILogger<EstimationsController> logger)
        {
            _commandProcessor = commandProcessor;
            _stateHolderService = stateHolderService;
            _logger = logger;
        }

        [HttpGet("{ticket}")]
        public async Task<ActionResult<EstimationState>> GetEstimationState(Guid ticket)
        {
            var stateResponse = await _stateHolderService.StateQueryAsync(
                new StateRequest { Ticket = ticket.ToString("D") });

            return Ok(new EstimationState
            {
                Ticket = stateResponse.Ticket,
                State = stateResponse.State
            });
        }

        [HttpPost]
        public ActionResult<NewEstimationResponse> ProcessNewEstimation([FromBody]NewEstimationRequest request)
        {
            var message = new EstimationRequestedEvent
            {
                CaseNumber = Guid.NewGuid(),
                ClientId = "From auth",
                CallbackUri = request.CallBackUri,
                ImageUrls = request.ImageUrls
            };

            _commandProcessor.Post(message);

            return Accepted(new NewEstimationResponse { Token = message.CaseNumber.ToString("D") });
        }
    }
}
