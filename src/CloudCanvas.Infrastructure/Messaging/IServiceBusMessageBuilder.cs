using Azure.Messaging.ServiceBus;

namespace CloudCanvas.Infrastructure.Messaging
{
    public interface IServiceBusMessageBuilder
    {
        IServiceBusMessageBuilder WithSubject(string subject);
        IServiceBusMessageBuilder AddProperty(string key, object value);
        IServiceBusMessageBuilder SetCorrelationId(string correlationId);
        IServiceBusMessageBuilder AddProperties(IDictionary<string, object> props);
        ServiceBusMessage Finalize(string? messageId = null);
    }
}
