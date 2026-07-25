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


        IMessageBuilder ProjectionCompleteMessage(string src_container, string correlationId);
        IMessageBuilder ThumbnailsCreatedMessage(string? correlationId);
    }
}
