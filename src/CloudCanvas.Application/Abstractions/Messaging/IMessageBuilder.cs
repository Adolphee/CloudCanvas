using CloudCanvas.Application.Events;

namespace CloudCanvas.Application.Abstractions.Messaging
{
    public interface IMessageBuilder
    {
        IMessageBuilder WithSubject(string subject);
        IMessageBuilder AddProperty(string key, object value);
        IMessageBuilder SetCorrelationId(string? correlationId = null);
        IMessageBuilder AddProperties(IDictionary<string, object> props);
        CCEventMessage Finalize(string? messageId = null);
        IMessageBuilder CreateThumbnailsMessage(string? correlationId = null);
        IMessageBuilder ThumbnailsCreationComplete(string? correlationId = null);
    }
}
