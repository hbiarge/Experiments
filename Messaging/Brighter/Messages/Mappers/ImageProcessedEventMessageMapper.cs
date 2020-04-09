using System.Text.Json;
using Paramore.Brighter;
using Shared;

namespace Messages.Mappers
{
    public class ImageProcessedEventMessageMapper : IAmAMessageMapper<ImageProcessedEvent>
    {
        public Message MapToMessage(ImageProcessedEvent request)
        {
            var header = new MessageHeader(
                messageId: request.Id,
                topic: Constants.Queues.ProcessManager,
                messageType: MessageType.MT_EVENT);
            var body = new MessageBody(JsonSerializer.Serialize(request));
            var message = new Message(header, body);
            return message;
        }

        public ImageProcessedEvent MapToRequest(Message message)
        {
            var greetingCommand = JsonSerializer.Deserialize<ImageProcessedEvent>(message.Body.Value);

            return greetingCommand;
        }
    }
}