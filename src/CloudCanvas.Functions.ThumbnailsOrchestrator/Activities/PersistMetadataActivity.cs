using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Functions.ThumbnailOrchestrator.DTO;
using CloudCanvas.Infrastructure.Cosmos;
using CloudCanvas.Infrastructure.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.Activities;

public class PersistMetadataActivity(IPostsRepository<IPost> cosmos)
{
    private readonly IPostsRepository<IPost> _cosmos = cosmos;

    [Function(nameof(PersistMetadataActivity))]
    public async Task<BlobMetadata> Run([ActivityTrigger] RequestContext req, FunctionContext context)
    {
        var logger = context.GetLogger<PersistMetadataActivity>();
        try
        {
            req.Blob.ProcessingStage = (int)BlobProcessingStage.UpdateMetadata;
            req.Blob.LastModified = DateTimeOffset.Now;
            await _cosmos.SaveMetadataAsync(req.Blob.ToPhoto(), CloudCosmos.Containers.BlobMeta, true);      // Overwrite metadata to CosmosDB
            logger.LogInformation("{correlationId} Metadata Persisted for blob {container}/{identifier}", req.CorrelationId, req.Blob.ContainerName, req.Blob.Name);
        }
        catch (Exception e) when (e is CCSerializationException || e is InvalidArgumentException)
        {
            logger.LogError(e, "{correlationId} Failed to deserialize metadata into an object of type {type}. Operation aborted. ", req.CorrelationId, nameof(BlobMetadata));
            throw;
        }
        return req.Blob;
    }
}