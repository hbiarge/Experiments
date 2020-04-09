using Rebus.Config;
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;
using Rebus.Pipeline.Send;

namespace Acheve.Common.Messages.Tracing
{
    public static class TraceConfigurationExtensions
    {
        public static void UseDistributeTracingFlow(this OptionsConfigurer configurer)
        {
            configurer.Decorate<IPipeline>(c =>
            {
                var outgoingStep = new SetTraceParentOutgoingStep();
                var incomingStep = new GetTraceParentIncomingStep();

                var pipeline = c.Get<IPipeline>();

                return new PipelineStepInjector(pipeline)
                    .OnReceive(incomingStep, PipelineRelativePosition.After, typeof(DeserializeIncomingMessageStep))
                    .OnSend(outgoingStep, PipelineRelativePosition.Before, typeof(SerializeOutgoingMessageStep));
            });
        }
    }
}