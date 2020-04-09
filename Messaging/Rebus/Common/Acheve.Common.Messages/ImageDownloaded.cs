using System;

namespace Acheve.Common.Messages
{
    public class ImageDownloaded
    {
        public Guid CaseNumber { get; set; }

        public int ImageId { get; set; }

        public string ImageTicket { get; set; }
    }
}