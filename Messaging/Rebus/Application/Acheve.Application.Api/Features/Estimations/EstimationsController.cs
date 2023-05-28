using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Acheve.Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using StateHolder;

namespace Acheve.Application.Api.Features.Estimations
{
    [ApiController]
    [Route("[controller]")]
    public class EstimationsController : ControllerBase
    {
        private readonly IBus _bus;
        private readonly StateHolderService.StateHolderServiceClient _stateHolderService;
        private readonly ILogger<EstimationsController> _logger;

        public EstimationsController(
            IBus bus,
            StateHolderService.StateHolderServiceClient stateHolderService,
            ILogger<EstimationsController> logger)
        {
            _bus = bus;
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
        public async Task<ActionResult<NewEstimationResponse>> ProcessNewEstimation([FromBody]NewEstimationRequest request)
        {
            var currentActivity = Activity.Current;

            var message = new EstimationRequested
            {
                CaseNumber = Guid.NewGuid(),
                ClientId = "From auth",
                CallbackUri = request.CallBackUri,
                ImageUrls = request.ImageUrls
            };

            await _bus.Send(message);

            return Accepted(new NewEstimationResponse 
            { 
                Token = message.CaseNumber.ToString("D"),
                OperationId = currentActivity.RootId
            });
        }
    }
}
