using Azure.Messaging.ServiceBus;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Utilities;

namespace CloudCanvas.Shared.Services
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
            Validate.StringValue(nameof(payload), payload);
            _message = new ServiceBusMessage(payload);
        }

        public SBMessageBuilder(object payload)
        {
            Validate.Object(payload);
            _message = new ServiceBusMessage(CCSerializer.Serialize(payload));
        }

        public IServiceBusMessageBuilder SetCorrelationId(string? correlationId = null)
        {
            _message.CorrelationId = correlationId ?? Guid.NewGuid().ToString();
            return this;
        }

        public IServiceBusMessageBuilder WithSubject(string subject)
        {
            if(Validate.StringValue(nameof(subject), subject))
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
            Validate.StringValue(nameof(propertyName), propertyName);
            Validate.Object(propertyValue);
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
