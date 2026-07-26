using CloudCanvas.Application.Abstractions.Messaging;
using CloudCanvas.Application.Common;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Events;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Common.Enums;
using static CloudCanvas.Application.Common.Constants.ServiceBus;

namespace CloudCanvas.Infrastructure.Messaging
{
    public class SBMessageBuilder : IMessageBuilder
    {
        private readonly CCEventMessage _message;
        public SBMessageBuilder()
        {
            _message = new CCEventMessage();
        }

        public SBMessageBuilder(PhotoDTO payload)
        {
            _message = new()
            {
                Payload = BinaryData.FromObjectAsJson(payload),
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

        public IMessageBuilder ProjectionCompleteMessage(string srcContainer = BStorage.Containers.Uploads, string? correlationId = default)
        {
            if (_message.Payload is null) throw new InvalidOperationException("No payload available.");
            return WithSubject($"{Status.MetadataPersisted} - Ready for thumbnails.")    // Add Subject
                .AddProperty(Props.ContainerName, srcContainer) // Original file's upload-container
                .AddProperty(Props.EventType, Subs.PersistMetadata) // So that it makes it through subscription filters
                .AddProperty(Props.ThumbnailSize, (int)ThumbnailSize.small) // BuildForPhoto thumbnail generation, later used by orchestrators to fan-out differet sizes
                .SetCorrelationId(correlationId); // Set a new CorrelationId for this message, as the first in the chain
        }

        public IMessageBuilder ThumbnailsCreatedMessage(string? correlationId = default)
        {
            if(_message.Payload is null) throw new InvalidOperationException("No payload available.");
            return WithSubject(Status.OrchestrationFinished)
               .SetCorrelationId(correlationId)
                .AddProperty(Props.EventType, Subs.CreateThumbnail) // So that it makes it through subscription filters
               .AddProperty(BStorage.Meta.CompletedOn, DateTimeOffset.Now.ToString());
        }
    }
}
