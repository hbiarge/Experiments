using System;

namespace Acheve.Common.Messages
{
    public class AwaitExternalImageToBeProcessed
    {
        public AwaitExternalImageToBeProcessed(Guid caseNumber, int imageId)
        {
            CaseNumber = caseNumber;
            ImageId = imageId;
        }

        public Guid CaseNumber { get; }

        public int ImageId { get; }
    }
}