using System;

namespace Acheve.Common.Messages
{
    public class EstimationError
    {
        public Guid CaseNumber { get; set; }

        public string CallbackUri { get; set; }

        public string Reason { get; set; }
    }
}