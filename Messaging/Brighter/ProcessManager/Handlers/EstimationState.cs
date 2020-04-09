using System;
using System.Collections.Generic;
using Messages;
using Rebus.Sagas;

namespace ProcessManager.Handlers
{
    public class EstimationState : SagaData
    {
        public Guid CaseNumber { get; set; }

        public string ClientId { get; set; }

        public string CallbackUrl { get; set; }

        public ICollection<CaseImage> Images { get; set; }

        public EstimationStates State { get; set; }

        public string EstimationTicket { get; set; }
        
        public string EstimationError { get; set; }
    }
}