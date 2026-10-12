using CloudCanvas.Application.Thumbnails.Commands.CreateThumbnail;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.Activities;

public sealed class CreateThumbnailActivity(ISender sender)
{
    private readonly ISender _sender = sender;

    [Function(nameof(CreateThumbnailActivity))]
    public async Task<string> Run([ActivityTrigger] CreateThumbnailActivityRequest req, FunctionContext context)
    {
        var logger = context.GetLogger(nameof(CreateThumbnailActivity));
        logger.LogInformation("{correlationId} Activity Invoked: Create {size} for {containerName}/{photoIdentifier}", 
        req.CorrelationId, req.thumbnailSize, req.SrcContainer, req.Photo.Id);
        var cmd = new CreateThumbnailCommand(req.Photo, req.thumbnailSize, req.SrcContainer, req.CorrelationId);
        var thumbnail = await _sender.Send(cmd);
        return thumbnail.ThumbnailUrl;
    }
}