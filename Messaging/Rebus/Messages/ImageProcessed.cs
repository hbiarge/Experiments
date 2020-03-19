using System;
using Rebus.DataBus;

namespace Messages
{
    public class ImageProcessed
    {
        public Guid CaseNumber { get; set; }

        public int ImageId { get; set; }

        public string MetadataTicket { get; set; }
    }
}