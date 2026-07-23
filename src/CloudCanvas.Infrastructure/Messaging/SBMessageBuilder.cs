using CloudCanvas.Application.Abstractions.Messaging;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Events;
using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Infrastructure.Messaging
{
    public class SBMessageBuilder : IMessageBuilder, IMessageBuilderExtensions
    {
        private readonly CCEventMessage _message;
        public SBMessageBuilder()
        {
            _message = new CCEventMessage();
        }

        public SBMessageBuilder(object payload)
        {
            _message = new()
            {
                Payload = payload
            };
        }

        public IMessageBuilder SetCorrelationId(string? correlationId = null)
        {
            _message.CorrelationId = correlationId ?? Guid.NewGuid().ToString();
            return this;
        }

        public IMessageBuilder WithSubject(string subject)
        {
            _message.Subject = subject;
            return this;
        }

        public IMessageBuilder AddProperties(IDictionary<string, object> props)
        {
            foreach (var (key, value) in props) AddProperty(key, value);
            return this;
        }

        public IMessageBuilder AddProperty(string propertyName, object propertyValue)
        {
            _message.Properties.Add(propertyName, propertyValue);
            return this;
        }

        public CCEventMessage Finalize(string? messageId = null)
        {
            _message.Id = messageId ?? Guid.NewGuid().ToString();
            return _message;
        }

        public IMessageBuilder CreateThumbnailsMessage(string? correlationId)
        {
            if (_message.Payload is null) throw new InvalidOperationException("No payload available.");
            return WithSubject($"{ServiceBus.Status.MetadataPersisted} - Ready for thumbnails.")    // Add Subject
                .AddProperty(ServiceBus.Props.EventType, ServiceBus.Subs.PersistMetadata) // So that it makes it through subscription filters
                .AddProperty(ServiceBus.Props.ThumbnailSize, (int)ThumbnailSize.small) // BuildFor thumbnail generation, later used by orchestrators to fan-out differet sizes
                .SetCorrelationId(correlationId); // Set a new CorrelationId for this message, as the first in the chain
        }

        public IMessageBuilder ThumbnailsCreationComplete(string? correlationId)
        {
            if(_message.Payload is null) throw new InvalidOperationException("No payload available.");
            return WithSubject(ServiceBus.Status.OrchestrationFinished)
               .SetCorrelationId(correlationId)
               .AddProperty(BStorage.Meta.CompletedOn, DateTimeOffset.Now);
        }
    }
}
