using CloudCanvas.Application.Abstractions.Messaging;
using CloudCanvas.Application.Posts.DTOs;

namespace CloudCanvas.Infrastructure.Messaging
{
    public class MessageFactory: IMessageFactory
    {
        public IMessageBuilder BuildForPhoto(PhotoDTO photo)
        {
            return new SBMessageBuilder(photo);
        }
    }
}
