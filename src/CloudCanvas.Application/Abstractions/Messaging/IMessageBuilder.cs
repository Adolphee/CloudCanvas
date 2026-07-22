using Azure.Messaging.ServiceBus;

namespace CloudCanvas.Application.Abstractions.Messaging
{
    public interface IMessageBuilder
    {
        IMessageBuilder WithSubject(string subject);
        IMessageBuilder AddProperty(string key, object value);
        IMessageBuilder SetCorrelationId(string correlationId);
        IMessageBuilder AddProperties(IDictionary<string, object> props);
        IMessageBuilder CreateThumbnailsMessage(string correlationId);
        ServiceBusMessage Finalize(string? messageId = null);
    }
}
