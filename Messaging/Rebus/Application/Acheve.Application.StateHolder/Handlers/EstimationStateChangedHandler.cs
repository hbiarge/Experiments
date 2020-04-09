using System.Threading.Tasks;
using Acheve.Common.Messages;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Acheve.Application.StateHolder.Handlers
{
    public class EstimationStateChangedHandler : IHandleMessages<EstimationStateChanged>
    {
        private readonly State _state;
        private readonly ILogger<EstimationStateChangedHandler> _logger;

        public EstimationStateChangedHandler(State state, ILogger<EstimationStateChangedHandler> logger)
        {
            _state = state;
            _logger = logger;
        }

        public Task Handle(EstimationStateChanged message)
        {
            _logger.LogInformation(
                "Receiving state change for case number {caseNumber}. New state: {state}",
                message.CaseNumber,
                message.State);

            _state.AddOrUpdateState(message.CaseNumber.ToString("D"), message.State);

            return Task.CompletedTask;
        }
    }
}
