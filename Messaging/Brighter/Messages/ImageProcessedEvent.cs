using System;
using Paramore.Brighter;

namespace Messages
{
    public class ImageProcessedEvent : Event
    {
        public ImageProcessedEvent() : base(Guid.NewGuid())
        {
        }

        public Guid CaseNumber { get; set; }

        public int ImageId { get; set; }

        public string MetadataTicket { get; set; }
    }
}