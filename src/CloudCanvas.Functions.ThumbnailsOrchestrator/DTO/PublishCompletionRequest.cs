using CloudCanvas.Infrastructure.DTOs;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public sealed class PublishCompletionRequest(BlobMetadata blob, string correlationId, string instanceId) 
        : PersistMetadataActivityRequest(blob, correlationId, instanceId)
    {
        public PublishCompletionRequest(ThumbnailActivityRequest req)
            : this(req.Blob, req.CorrelationId, req.InstanceId) { }
    }
}
