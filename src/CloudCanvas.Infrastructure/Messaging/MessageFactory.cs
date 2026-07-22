using CloudCanvas.Application.Abstractions.Messaging;

namespace CloudCanvas.Infrastructure.Messaging
{
    public class MessageFactory: IMessageFactory
    {
        public IMessageBuilder BuildFor(object payload)
        {
            return new SBMessageBuilder(payload);
        }
    }
}
