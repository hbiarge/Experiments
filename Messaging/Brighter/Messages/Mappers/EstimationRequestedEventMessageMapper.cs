using System.Text.Json;
using Messages.Events;
using Paramore.Brighter;
using Shared;

namespace Messages.Mappers
{
    public class EstimationRequestedEventMessageMapper : IAmAMessageMapper<EstimationRequestedEvent>
    {
        public Message MapToMessage(EstimationRequestedEvent request)
        {
            var header = new MessageHeader(
                messageId: request.Id,
                topic: Constants.Queues.ProcessManager,
                messageType: MessageType.MT_EVENT);
            var body = new MessageBody(JsonSerializer.Serialize(request));
            var message = new Message(header, body);
            return message;
        }

        public EstimationRequestedEvent MapToRequest(Message message)
        {
            var greetingCommand = JsonSerializer.Deserialize<EstimationRequestedEvent>(message.Body.Value);

            return greetingCommand;
        }
    }
}
