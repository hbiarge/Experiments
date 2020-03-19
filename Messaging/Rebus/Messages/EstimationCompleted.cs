using System;

namespace Messages
{
    public class EstimationCompleted
    {
        public Guid CaseNumber { get; set; }

        public string EstimationTicket { get; set; }
    }
}