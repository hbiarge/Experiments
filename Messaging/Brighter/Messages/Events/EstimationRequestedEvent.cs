using System;
using System.Collections.Generic;
using Paramore.Brighter;

namespace Messages.Events
{
    public class EstimationRequestedEvent : Event
    {
        public EstimationRequestedEvent() : base(Guid.NewGuid())
        {
        }

        public Guid CaseNumber { get; set; }

        public string ClientId { get; set; }

        public string CallbackUri { get; set; }

        public ICollection<string> ImageUrls { get; set; }
    }
}
