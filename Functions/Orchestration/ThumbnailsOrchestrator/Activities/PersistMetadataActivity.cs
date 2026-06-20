using CloudCanvas.Functions.Orchestration.DTO;
using CloudCanvas.Functions.Orchestration.DTOs;
using CloudCanvas.Shared;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Enums;
using CloudCanvas.Shared.Exceptions;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Functions.Orchestration.Activities;

public class PersistMetadataActivity(CosmosClientWrapper cosmos)
{
    private readonly CosmosClientWrapper _cosmos = cosmos;

    [Function(nameof(PersistMetadataActivity))]
    public async Task<BlobMetaDTO> Run([ActivityTrigger] RequestContext req, FunctionContext context)
    {
        var logger = context.GetLogger<PersistMetadataActivity>();
        try
        {
            req.Blob.ProcessingStage = (int)BlobProcessingStage.UpdateMetadata;
            req.Blob.LastModified = DateTimeOffset.Now;
            await _cosmos.SaveMetadataAsync(req.Blob, CloudCosmos.Containers.BlobMeta, true);      // Overwrite metadata to CosmosDB
            logger.LogInformation("{correlationId} Metadata Persisted for blob {container}/{identifier}", req.CorrelationId, req.Blob.ContainerName, req.Blob.Name);
        }
        catch (Exception e) when (e is CCSerializationException || e is InvalidArgumentException)
        {
            logger.LogError(e, "{correlationId} Failed to deserialize metadata into an object of type {type}. Operation aborted. ", req.CorrelationId, nameof(BlobMetaDTO));
            throw;
        }
        return req.Blob;
    }
}