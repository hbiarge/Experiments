using System;

namespace Acheve.Common.Messages
{
    public class EstimationCompleted
    {
        public Guid CaseNumber { get; set; }

        public string EstimationTicket { get; set; }
    }
}