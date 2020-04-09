namespace Acheve.Application.ProcessManager.Messages
{
    public class VerifyComplete
    {
        public VerifyComplete(string caseNumber)
        {
            CaseNumber = caseNumber;
        }

        public string CaseNumber { get; }
    }
}
