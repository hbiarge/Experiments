using System;

namespace Acheve.Common.Messages
{
    public class UnableToEstimate
    {
        public Guid CaseNumber { get; set; }
        
        public string Error { get; set; }
    }
}