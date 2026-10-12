using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Application.Thumbnails.Commands.SaveThumbnail;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.Activities;

public class SaveThumbnailsActivity(ISender sender)
{
    private readonly ISender _sender = sender;

    [Function(nameof(SaveThumbnailsActivity))]
    public async Task<PhotoDTO> Run([ActivityTrigger] SaveThumbnailsActivityRequest req, CancellationToken cancellation = default)
    {
        var res = await _sender.Send(new SaveThumbnailsCommand
        {
            Photo = req.Photo,
            Creator = req.Photo.Creator,
        }, cancellation);
        return res.Photo!;
    }
}