using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Domain.Enums;

namespace CloudCanvas.Application.Thumbnails.Commands.CreateThumbnail
{
    public sealed record CreateThumbnailCommand(PhotoDTO Photo, ThumbnailSize ThumbnailSize, string OriginalContainer = BStorage.Containers.Uploads, string correlationId = null!): IRequest<CreateThumbnailResult>
    {
    }
}
