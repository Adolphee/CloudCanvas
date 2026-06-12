using CloudCanvas.Functions.Orchestration.DTOs;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Functions.Orchestration.Activities;

public class CreateThumbnailActivity
{
    private readonly BlobStorageService _blobService;

    public CreateThumbnailActivity(BlobStorageService blobStorageService)
    {
        _blobService = blobStorageService;
    }

    [Function(nameof(CreateThumbnailActivity))]
    public async Task<string> Run([ActivityTrigger] RequestContext req, FunctionContext context)
    {
        var logger = context.GetLogger(nameof(CreateThumbnailActivity));
        logger.LogInformation("{correlationId} Activity Invoked: Create {size} for {containerName}/{identifier}", 
            req.CorrelationId, req.ThumbnailSize, req.Blob.ContainerName, req.Blob.Name);
        try
        {
            ///TODO: Implement **better validation** on file type before CloudCanvas v1.0, 
            /// for example, what if this function receives a .pdf file? or a .mp4, .zip etc...
            var bclient = await _blobService.GetOrCreateContainerClientAsync(req.Blob.ContainerName); // original file blob container
            var stream = await bclient.GetBlobClient(req.Blob.Name).OpenReadAsync(); // download file
            using var thumbnail = await ImageTool.ResizeAsync(stream, req.ThumbnailSize); // Create thumbnail
            var props = BlobStorageService.SetOriginalMetadata(req.Blob.OriginalFilename, req.Blob.UploadedBy!);
            BlobMetaDTO thumbnailMeta = await _blobService.UploadAsync(thumbnail, req.Blob.OriginalFilename, props ,BlobStorage.Containers.Thumbnails, $"{req.Blob.Name}_{req.ThumbnailSize.ToString()}");
            logger.LogInformation("{correlationId} Created {size} thumbnail for {containerName}/{identifier}", 
                req.CorrelationId, req.ThumbnailSize, req.Blob.ContainerName, req.Blob.Name);
            return thumbnailMeta.Url.ToString();
        }
        catch (Exception e)
        { // Drop the ball
            logger.LogError(e, "{correlationId} Failed to create '{size}' thumbnail. ErrMessage:\n{errMessage}", req.CorrelationId, req.ThumbnailSize, e.Message);
            throw;
        }
    }
}