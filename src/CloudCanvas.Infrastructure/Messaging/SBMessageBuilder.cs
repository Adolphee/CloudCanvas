using Azure.Messaging.ServiceBus;
using CloudCanvas.Infrastructure.Common;

namespace CloudCanvas.Infrastructure.Messaging
{
    public class SBMessageBuilder : IServiceBusMessageBuilder
    {
        private readonly ServiceBusMessage _message;

        public SBMessageBuilder()
        {
            _message = new ServiceBusMessage();
        }

        public SBMessageBuilder(string payload)
        {   
            _message = new ServiceBusMessage(payload);
        }

        public SBMessageBuilder(object payload)
        {
            _message = new ServiceBusMessage(CCSerializer.Serialize(payload));
        }

        public IServiceBusMessageBuilder SetCorrelationId(string? correlationId = null)
        {
            _message.CorrelationId = correlationId ?? Guid.NewGuid().ToString();
            return this;
        }

        public IServiceBusMessageBuilder WithSubject(string subject)
        {
            _message.Subject = subject;
            return this;
        }

        public IServiceBusMessageBuilder AddProperties(IDictionary<string, object> props)
        {
            foreach (var (key, value) in props) AddProperty(key, value);
            return this;
        }

        public IServiceBusMessageBuilder AddProperty(string propertyName, object propertyValue)
        {
            _message.ApplicationProperties[propertyName] = propertyValue;
            return this;
        }

        public ServiceBusMessage Finalize(string? messageId = null)
        {
            _message.MessageId = messageId ?? Guid.NewGuid().ToString();
            return _message;
        }

    }
}
