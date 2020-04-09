using System;

namespace Acheve.Common.Messages
{
    public class UnableToProcessImage
    {
        public Guid CaseNumber { get; set; }

        public int ImageId { get; set; }
        
        public string Error { get; set; }
    }
}