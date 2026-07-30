using CloudCanvas.Application.Posts.Photos.Interfaces;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Application.Thumbnails.Commands.SaveThumbnail
{
    public sealed class SaveThumbnailsCommandHandler(IPhotoRepository repository, IPhotoProjectionStore projectionStore, ILogger<SaveThumbnailsCommandHandler> logger) : IRequestHandler<SaveThumbnailsCommand, SaveThumbnailsResult>
    {
        private readonly IPhotoRepository _repository = repository;
        private readonly IPhotoProjectionStore _projection = projectionStore;
        private readonly ILogger<SaveThumbnailsCommandHandler> _logger = logger; ///TODO: Add logging to this handler for better traceability and debugging.

        public async Task<SaveThumbnailsResult> Handle(SaveThumbnailsCommand command, CancellationToken cancellation = default)
        {
            var photo = await _repository.GetByIdAsync(command.Photo.Id!, cancellation) ?? throw new SaveThumbnailException("OriginalPhoto is missing.");
            photo.Thumbnails = command.Photo.ToPhoto().Thumbnails; // use case boundary is 'thumbnails' here
            try
            {
                if (await _repository.UpdateAsync(photo, cancellation))
                {
                    var ops = new Dictionary<string, object> { ["/thumbnails"] = command.Photo.Thumbnails };
                    var projection = await _projection.PatchAsync(new(photo.Id!, photo.UserId), ops, cancellation);
                    return new SaveThumbnailsResult(projection);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "EF Failed to save {ThumbnailsCount} thumbnails for ({PhotoId}).", command.Photo.Thumbnails.Count, photo.Id);
            }
            return new(null);
        }
    }
}
