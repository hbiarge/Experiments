using System;

namespace Messages
{
    public class ImageDownloaded
    {
        public Guid CaseNumber { get; set; }

        public int ImageId { get; set; }

        public string ImageTicket { get; set; }
    }
}