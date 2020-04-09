using System;

namespace Acheve.Common.Messages
{
    public class ImageProcessed
    {
        public Guid CaseNumber { get; set; }

        public int ImageId { get; set; }

        public string MetadataTicket { get; set; }
    }
}