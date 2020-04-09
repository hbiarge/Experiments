using System;
using Paramore.Brighter;

namespace Messages
{
    public class EstimationCompletedEvent : Event
    {
        public EstimationCompletedEvent() : base(Guid.NewGuid())
        {
        }

        public Guid CaseNumber { get; set; }

        public string EstimationTicket { get; set; }
    }
}