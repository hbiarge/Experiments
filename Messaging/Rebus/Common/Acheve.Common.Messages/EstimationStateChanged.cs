using System;

namespace Acheve.Common.Messages
{
    public class EstimationStateChanged
    {
        public Guid CaseNumber { get; set; }

        public EstimationStates State { get; set; }
    }
}