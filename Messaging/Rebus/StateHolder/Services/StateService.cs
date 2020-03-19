using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace StateHolder
{
    public class StateService : StateHolderService.StateHolderServiceBase
    {
        private readonly State _state;
        private readonly ILogger<StateService> _logger;

        public StateService(State state, ILogger<StateService> logger)
        {
            _state = state;
            _logger = logger;
        }

        public override Task<StateResponse> StateQuery(StateRequest request, ServerCallContext context)
        {
            var currentState = _state.GetState(request.Ticket);

            return Task.FromResult(new StateResponse
            {
                Ticket = request.Ticket,
                State = currentState
            });
        }
    }
}
