using CloudCanvas.Application.Thumbnails.Commands.CreateThumbnail;
using CloudCanvas.Functions.ThumbnailOrchestrator.DTO;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.Activities;

public sealed class CreateThumbnailActivity(ISender sender)
{
    private readonly ISender _sender = sender;

    [Function(nameof(CreateThumbnailActivity))]
    public async Task<string> Run([ActivityTrigger] RequestContext req, FunctionContext context)
    {
        var logger = context.GetLogger(nameof(CreateThumbnailActivity));
        logger.LogInformation("{correlationId} Activity Invoked: Create {size} for {containerName}/{photoIdentifier}", 
        req.CorrelationId, req.ThumbnailSize, req.ContainerName, req.Photo.Id);
        var cmd = new CreateThumbnailCommand(req.Photo, req.ThumbnailSize);
        return (await _sender.Send(cmd)).ThumbnailUrl;
    }
}