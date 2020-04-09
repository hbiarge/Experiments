namespace Acheve.Common.Messages.Tracing
{
    internal class TracingConstants
    {
        public const string ProducerActivityName = "Rebus.Diagnostics.Producer";
        public const string ConsumerActivityName = "Rebus.Diagnostics.Consumer";

        public const string TraceParentHeaderName = "x-trace-parent";
        public const string TraceStateHeaderName = "x-trace-state";
        public const string RequestIdHeaderName = "x-request-id";
    }
}