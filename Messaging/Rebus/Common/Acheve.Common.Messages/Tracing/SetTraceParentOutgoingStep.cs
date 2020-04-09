using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Rebus.Messages;
using Rebus.Pipeline;

namespace Acheve.Common.Messages.Tracing
{
    public class SetTraceParentOutgoingStep : IOutgoingStep
    {
        public async Task Process(OutgoingStepContext context, Func<Task> next)
        {
            var activity = StartActivity(context);
            InjectHeaders(activity, context);

            try
            {
                await next();
            }
            finally
            {
                StopActivity(activity, context);
            }
        }

        private static Activity StartActivity(OutgoingStepContext context)
        {
            var activity = new Activity(TracingConstants.ProducerActivityName);

            activity.Start();

            return activity;
        }

        private static void InjectHeaders(Activity activity, OutgoingStepContext context)
        {
            var message = context.Load<Message>();
            var headers = message.Headers;

            if (activity.IdFormat == ActivityIdFormat.W3C)
            {
                if (!headers.ContainsKey(TracingConstants.TraceParentHeaderName))
                {
                    headers[TracingConstants.TraceParentHeaderName] = activity.Id;

                    if (activity.TraceStateString != null)
                    {
                        headers[TracingConstants.TraceStateHeaderName] = activity.TraceStateString;
                    }
                }
            }
            else
            {
                if (!headers.ContainsKey(TracingConstants.RequestIdHeaderName))
                {
                    headers[TracingConstants.RequestIdHeaderName] = activity.Id;
                }
            }
        }

        private static void StopActivity(Activity activity, OutgoingStepContext context)
        {
            if (activity.Duration == TimeSpan.Zero)
            {
                activity.SetEndTime(DateTime.UtcNow);
            }

            activity.Stop();
        }
    }
}
