using CloudCanvas.Application.Posts.Galleries.Interfaces;
using CloudCanvas.Application.Posts.Photos.Interfaces;
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
            await _repository.SaveAsync(gallery, cancellationToken);
            var projection = await _store.CreateProjectionAsync(gallery.ToProjection(command.Creator!), cancellationToken);
            _logger.LogInformation("Gallery created with ID: {GalleryId}", gallery.Id);
            return new() { Gallery = projection };
        }
    }
}
