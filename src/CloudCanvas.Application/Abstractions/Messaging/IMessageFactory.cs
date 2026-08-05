using CloudCanvas.Application.Posts.Photos;

namespace CloudCanvas.Application.Abstractions.Messaging
{
    public interface IMessageFactory
    {
        IMessageBuilder BuildForPhoto(PhotoDTO payload);
    }
}
