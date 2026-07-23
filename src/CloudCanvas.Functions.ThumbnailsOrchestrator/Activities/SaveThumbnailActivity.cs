using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Thumbnails.Commands.SaveThumbnail;
using CloudCanvas.Functions.ThumbnailOrchestrator.DTO;
using MediatR;
using Microsoft.Azure.Functions.Worker;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.Activities;

public class SaveThumbnailActivity(ISender sender)
{
    private readonly ISender _sender = sender;

    [Function(nameof(SaveThumbnailActivity))]
    public async Task<SaveThumbnailResult> Run([ActivityTrigger] RequestContext req, FunctionContext context, CancellationToken cancellation = default)
    {
        return await _sender.Send(new SaveThumbnailCommand
        {
            Photo = req.Photo,
            creator = new Creator()
            {
                Id = req.Photo.Creator?.Id,
                UserName = req.Photo.Creator?.UserName,
                DisplayName = req.Photo.Creator?.DisplayName
            },
            ThumbnailSize = req.ThumbnailSize,
            ThumbnailURL = req.ThumbnailURL
        }, cancellation);
    }
}