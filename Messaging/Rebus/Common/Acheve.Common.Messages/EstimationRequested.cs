using System;
using System.Collections.Generic;

namespace Acheve.Common.Messages
{
    public class EstimationRequested
    {
        public Guid CaseNumber { get; set; }

        public string ClientId { get; set; }

        public string CallbackUri { get; set; }

        public ICollection<string> ImageUrls { get; set; }
    }
}
