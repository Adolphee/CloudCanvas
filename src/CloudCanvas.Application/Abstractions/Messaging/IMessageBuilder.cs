using CloudCanvas.Application.Events;

namespace CloudCanvas.Application.Abstractions.Messaging
{
    public interface IMessageBuilder
    {
        IMessageBuilder WithSubject(string subject);
        IMessageBuilder AddProperty(string key, object value);
        IMessageBuilder SetCorrelationId(string? correlationId = null);
        IMessageBuilder SetSessionId(string? sessionId = null);
        IMessageBuilder AddProperties(IDictionary<string, object> props);
        CCEventMessage Finalize(string? messageId = null);


        IMessageBuilder ProjectionCompleteMessage(string correlationId);
        IMessageBuilder ThumbnailsCreatedMessage(string? correlationId);
        IMessageBuilder ReadyForEnrichmentMessage(string operation, string? correlationId = default);
    }
}
