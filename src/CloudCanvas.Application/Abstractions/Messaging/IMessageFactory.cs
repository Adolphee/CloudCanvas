namespace CloudCanvas.Application.Abstractions.Messaging
{
    public interface IMessageFactory
    {
        IMessageBuilder BuildForPhoto(PhotoDTO payload);
    }
}
