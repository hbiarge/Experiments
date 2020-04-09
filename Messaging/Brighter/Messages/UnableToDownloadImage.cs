using System;

namespace Messages
{
    public class UnableToDownloadImage
    {
        public Guid CaseNumber { get; set; }

        public int ImageId { get; set; }

        public string Error { get; set; }
    }
}