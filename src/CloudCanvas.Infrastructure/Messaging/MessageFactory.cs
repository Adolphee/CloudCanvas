using CloudCanvas.Application.Abstractions.Messaging;
using CloudCanvas.Application.Posts.Photos;

namespace CloudCanvas.Infrastructure.Messaging
{
    public class MessageFactory: IMessageFactory
    {
        public IMessageBuilder BuildForPhoto(PhotoDTO photo) => new SBMessageBuilder(photo);
        public IMessageBuilder BuildForEnrichment(EnrichmentTarget image) => new SBMessageBuilder(image);
    }
}
