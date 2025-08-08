using CloudCanvas.Shared.DTOs;
namespace CloudCanvas.Functions.Orchestration.DTO
{
    public class OrchestrationRequest(BlobMetaDTO blob, string correlationId, string instanceId) : InceptionRequest(blob, correlationId)
    {
        public string InstanceId { get; set; } = instanceId;
    }

}
