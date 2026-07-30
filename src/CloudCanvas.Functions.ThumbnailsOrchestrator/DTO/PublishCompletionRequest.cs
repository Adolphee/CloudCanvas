namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public sealed record PublishCompletionRequest(PhotoDTO photo, string correlationId, string instanceId) 
        : ThumbnailOrchestrationRequest(photo, default!, correlationId, instanceId)
    {
    }
}
