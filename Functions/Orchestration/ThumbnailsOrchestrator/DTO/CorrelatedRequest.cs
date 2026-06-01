using CloudCanvas.Functions.Orchestration.DTO;
using CloudCanvas.Shared.DTOs;

namespace CloudCanvas.Functions.Orchestration.DTO
{
    // I need this to pull correlation through to the individual activities
    // The aim is to track the blob proces and not let go of the correlationId at all
    public class CorrelatedRequest(string correlationId)
    {
        public string CorrelationId { get; } = correlationId;
    }
}
