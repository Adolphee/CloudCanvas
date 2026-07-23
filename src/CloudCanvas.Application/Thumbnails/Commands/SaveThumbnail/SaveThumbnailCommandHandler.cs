using CloudCanvas.Application.Common;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Domain.Thumbnail;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Application.Thumbnails.Commands.SaveThumbnail
{
    public sealed class SaveThumbnailCommandHandler(IPhotoRepository context, IPhotoProjectionStore projection, ILogger<SaveThumbnailCommandHandler> logger) : IRequestHandler<SaveThumbnailCommand, SaveThumbnailResult>
    {
        private readonly IPhotoRepository _context = context;
        private readonly IPhotoProjectionStore _projection = projection;
        private readonly ILogger<SaveThumbnailCommandHandler> _logger = logger; ///TODO: Add logging to this handler for better traceability and debugging.

        public async Task<SaveThumbnailResult> Handle(SaveThumbnailCommand command, CancellationToken cancellation)
        {
            var photo = await _context.GetByIdAsync(command.Photo.Id!, cancellation);
            if (photo != null)
            {
                var thumb = new PhotoThumbnail
                {
                    Size = command.ThumbnailSize,
                    Url = command.ThumbnailURL,
                    PhotoId = command.Photo.Id,
                    OriginalImageURL = command.Photo.OriginalFilename,
                };
                photo.Thumbnails.Add(thumb);
                if(await _context.UpdateAsync(photo, cancellation))
                {
                    return new SaveThumbnailResult
                    {
                        Status = CCOperationStatus.Success,
                        Photo = await _projection.SaveProjectionAsync(photo.ToProjection(command.creator), CloudCosmos.Containers.UserPhotos, default, cancellation),
                        Size = command.ThumbnailSize,
                        Location = command.ThumbnailURL
                    };
                }
                throw new SaveThumbnailException("Failed to save thumbnail.");
            }
            return new() { Status = CCOperationStatus.Failed, Photo = command.Photo };
        }
    }
}
