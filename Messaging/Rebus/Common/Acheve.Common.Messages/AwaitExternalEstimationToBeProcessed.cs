using System;

namespace Acheve.Common.Messages
{
    public class AwaitExternalEstimationToBeProcessed
    {
        public AwaitExternalEstimationToBeProcessed(Guid caseNumber)
        {
            CaseNumber = caseNumber;
        }

        public Guid CaseNumber { get; }
    }
}