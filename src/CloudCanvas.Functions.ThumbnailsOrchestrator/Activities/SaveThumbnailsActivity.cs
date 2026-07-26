using CloudCanvas.Application.Thumbnails.Commands.SaveThumbnail;
using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.Activities;

public class SaveThumbnailsActivity(ISender sender)
{
    private readonly ISender _sender = sender;

    [Function(nameof(SaveThumbnailsActivity))]
    public async Task<PhotoDTO> Run([ActivityTrigger] SaveThumbnailsActivityRequest req, FunctionContext context, CancellationToken cancellation = default)
    {
        await _sender.Send(new SaveThumbnailsCommand
        {
            Photo = req.photo,
            creator = new Creator()
            {
                Id = req.photo.Creator?.Id,
                UserName = req.photo.Creator?.UserName,
                DisplayName = req.photo.Creator?.DisplayName
            },
            OriginalContainer = req.srcContainer
        }, cancellation);
        return req.photo;
    }
}