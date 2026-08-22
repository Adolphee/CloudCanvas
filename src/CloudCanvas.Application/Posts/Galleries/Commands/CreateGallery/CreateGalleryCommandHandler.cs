using CloudCanvas.Application.Posts.Galleries.Interfaces;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Application.Common.Mapping;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Application.Posts.Galleries.Commands.CreateGallery
{
    public class CreateGalleryCommandHandler(IGalleryProjectionStore projectionStore, IGalleryRepository repository, IPhotoRepository _photoRepo, ILogger<CreateGalleryCommandHandler> logger) : IRequestHandler<CreateGalleryCommand, CreateGalleryResult>
    {
        private readonly IGalleryProjectionStore _store = projectionStore;
        private readonly IGalleryRepository _repository = repository;
        private readonly IPhotoRepository _photoRepository = _photoRepo;
        private readonly ILogger<CreateGalleryCommandHandler> _logger = logger;

        public async Task<CreateGalleryResult> Handle(CreateGalleryCommand command, CancellationToken cancellationToken)
        {
            var gallery = command.ToGallery(cancellationToken);
            var photos = await _photoRepository.GetPhotosByIdsAsync(command.Photos, cancellationToken);
            gallery.Photos = photos;
            var projection = gallery.ToProjection(command.Creator!.MinimalVersion());
            await _repository.SaveAsync(gallery, cancellationToken);
            projection = await _store.CreateProjectionAsync(projection, cancellationToken);
            _logger.LogInformation("Gallery created with ID: {GalleryId}", gallery.Id);
            return new() { Gallery = projection };
        }
    }
}
