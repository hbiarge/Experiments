using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Rebus.Messages;
using Rebus.Pipeline;

namespace Acheve.Common.Messages.Tracing
{
    public class GetTraceParentIncomingStep : IIncomingStep
    {
        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            var activity = StartActivity(context);

            try
            {
                await next();
            }
            finally
            {
                StopActivity(activity, context);
            }
        }

        private static Activity StartActivity(IncomingStepContext context)
        {
            var message = context.Load<Message>();
            var headers = message.Headers;

            var activity = new Activity(TracingConstants.ConsumerActivityName);
            
            if (headers.TryGetValue(TracingConstants.TraceParentHeaderName, out var requestId) == false)
            {
                headers.TryGetValue(TracingConstants.RequestIdHeaderName, out requestId);
            }

            if (string.IsNullOrEmpty(requestId) == false)
            {
                // This is the magic 
                activity.SetParentId(requestId);

                if (headers.TryGetValue(TracingConstants.TraceStateHeaderName, out var traceState))
                {
                    activity.TraceStateString = traceState;
                }
            }

            // The current activity gets an ID with the W3C format
            activity.Start();

            return activity;
        }

        private static void StopActivity(Activity activity, IncomingStepContext context)
        {
            if (activity.Duration == TimeSpan.Zero)
            {
                activity.SetEndTime(DateTime.UtcNow);
            }

            activity.Stop();
        }
    }
}