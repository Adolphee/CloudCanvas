using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Application.Thumbnails.Commands.CreateThumbnail
{
    public sealed record CreateThumbnailCommand(PhotoDTO Photo, ThumbnailSize ThumbnailSize, string OriginalContainer = BStorage.Containers.Uploads, string correlationId = null!): IRequest<CreateThumbnailResult>
    {
    }
}
