using System.Text.Json;
using Paramore.Brighter;
using Shared;

namespace Messages.Mappers
{
    public class EstimationCompletedEventMessageMapper : IAmAMessageMapper<EstimationCompletedEvent>
    {
        public Message MapToMessage(EstimationCompletedEvent request)
        {
            var header = new MessageHeader(
                messageId: request.Id,
                topic: Constants.Queues.ProcessManager,
                messageType: MessageType.MT_EVENT);
            var body = new MessageBody(JsonSerializer.Serialize(request));
            var message = new Message(header, body);
            return message;
        }

        public EstimationCompletedEvent MapToRequest(Message message)
        {
            var greetingCommand = JsonSerializer.Deserialize<EstimationCompletedEvent>(message.Body.Value);

            return greetingCommand;
        }
    }
}