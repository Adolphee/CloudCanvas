using CloudCanvas.Application.Abstractions.Messaging;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Events;
using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Domain.Enums;
using static CloudCanvas.Application.Common.Constants.ServiceBus;

namespace CloudCanvas.Infrastructure.Messaging
{
    public class SBMessageBuilder : IMessageBuilder
    {
        private CCEventMessage _message = default!;
        public SBMessageBuilder(PhotoDTO payload)
        {
            _message = new()
            {
                Payload = BinaryData.FromObjectAsJson(payload)
            };
            SetSessionId(payload.Id);
        }

        public SBMessageBuilder(){}
        public SBMessageBuilder(EnrichmentTarget target)
        {
            _message = new()
            {
                Payload = BinaryData.FromObjectAsJson(target)
            };
            SetSessionId(target.id);
        }

        public IMessageBuilder SetCorrelationId(string? correlationId = null)
        {
            GuardPayload();
            _message.CorrelationId = correlationId ?? Guid.NewGuid().ToString();
            return this;
        }

        public IMessageBuilder SetSessionId(string? sessionId = null)
        {
            GuardPayload();
            _message.SessionId = sessionId ?? Guid.NewGuid().ToString();
            return this;
        }

        public IMessageBuilder WithSubject(string subject)
        {
            GuardPayload();
            _message.Subject = subject;
            return this;
        }

        public IMessageBuilder AddProperties(IDictionary<string, object> props)
        {
            GuardPayload();
            foreach (var (key, value) in props) AddProperty(key, value);
            return this;
        }

        public IMessageBuilder AddProperty(string propertyName, object propertyValue)
        {
            GuardPayload();
            _message.Properties.Add(propertyName, propertyValue);
            return this;
        }

        public CCEventMessage Finalize(string? messageId = null)
        {
            GuardPayload();
            _message.Id = messageId ?? Guid.NewGuid().ToString();
            return _message;
        }

        public IMessageBuilder ProjectionCompleteMessage(string? correlationId = default)
        {
            GuardPayload();
            return WithSubject($"{Status.MetadataPersisted} - Ready for thumbnails.")    // Add Subject
                .AddProperty(Props.ContainerName, BStorage.Containers.Uploads) // Original file's upload-container
                .AddProperty(Props.EventType, Subs.PersistMetadata) // So that it makes it through subscription filters
                .AddProperty(Props.ThumbnailSize, (int)ThumbnailSize.small) // BuildForPhoto thumbnail generation, later used by orchestrators to fan-out differet sizes
                .SetCorrelationId(correlationId ?? Guid.NewGuid().ToString()); // Set a new CorrelationId for this message, as the first in the chain
        }

        public IMessageBuilder ThumbnailsCreatedMessage(string? correlationId = default)
        {
            GuardPayload();
            return WithSubject(Status.OrchestrationFinished)
               .SetCorrelationId(correlationId ?? Guid.NewGuid().ToString())
               .AddProperty(Props.EventType, Subs.CreateThumbnail) // So that it makes it through subscription filters
               .AddProperty(BStorage.Meta.CompletedOn, DateTimeOffset.Now.ToString());
        }

        public IMessageBuilder ReadyForEnrichmentMessage(string operation, string? correlationId = default)
        {
            GuardPayload();
            return WithSubject(Status.Intelligence)
               .SetCorrelationId(correlationId ?? Guid.NewGuid().ToString())
               .AddProperty(Props.Operation, operation) // trigger A.I. tagging
               .AddProperty(BStorage.Meta.CompletedOn, DateTimeOffset.Now.ToString());
        }

        private void GuardPayload()
        {
            if(_message.Payload is null) throw new InvalidOperationException("No payload available.");
        }
    }
}
