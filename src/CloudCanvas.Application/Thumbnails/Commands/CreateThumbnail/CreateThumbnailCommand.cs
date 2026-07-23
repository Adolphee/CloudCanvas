using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Application.Thumbnails.Commands.CreateThumbnail
{
    public sealed record CreateThumbnailCommand(PhotoDTO Photo, ThumbnailSize ThumbnailSize, string correlationId = null): IRequest<CreateThumbnailResult>
    {
        public string OriginalContainer { get; } = BStorage.Containers.Uploads; //Where to look for the original file
        public string _correlationId { get; set; } = correlationId ?? Guid.NewGuid().ToString();
    }
}
