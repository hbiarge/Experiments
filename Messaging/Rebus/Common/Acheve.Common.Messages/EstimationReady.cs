using System;

namespace Acheve.Common.Messages
{
    public class EstimationReady
    {
        public Guid CaseNumber { get; set; }

        public string CallbackUri { get; set; }
        
        public string EstimationId { get; set; }
    }
}