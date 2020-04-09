using System;

namespace Messages
{
    public class ImageUrlReceived
    {
        public Guid CaseNumber { get; set; }

        public int ImageId { get; set; }
        
        public string ImageUrl { get; set; }
    }
}