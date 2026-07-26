using CloudCanvas.Application.Common;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Application.Thumbnails.Commands.SaveThumbnail
{
    public sealed class SaveThumbnailsCommandHandler(IPhotoRepository context, IPhotoProjectionStore projection, ILogger<SaveThumbnailsCommandHandler> logger) : IRequestHandler<SaveThumbnailsCommand, SaveThumbnailsResult>
    {
        private readonly IPhotoRepository _context = context;
        private readonly IPhotoProjectionStore _projection = projection;
        private readonly ILogger<SaveThumbnailsCommandHandler> _logger = logger; ///TODO: Add logging to this handler for better traceability and debugging.

        public async Task<SaveThumbnailsResult> Handle(SaveThumbnailsCommand command, CancellationToken cancellation)
        {
            var photo = await _context.GetByIdAsync(command.Photo.Id!, cancellation);
            if (photo == null) throw new ArgumentNullException(nameof(photo));
            photo.Thumbnails = command.Photo.ToPhoto().Thumbnails; // respecting the responsability boundary 'thumbnails'

            try
            {
                if (await _context.UpdateAsync(photo, cancellation))
                {
                    var ops = new Dictionary<string, object> { ["/thumbnails"] = command.Photo.Thumbnails };
                    var projection = await _projection.PatchAsync(photo.Id, photo.UserId, Projection.Containers.UserPhotos, ops, cancellation);

                    return new SaveThumbnailsResult
                    {
                        Status = CCOperationStatus.Success,
                        Photo = projection
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"EF Failed to save {photo.Thumbnails.Count} thumbnails for ({photo.Id}).");
            }
            return new() { Status = CCOperationStatus.Failed, Photo = photo?.ToProjection(command.creator) ?? null };
        }
    }
}
