using CloudCanvas.Application.Abstractions.Messaging;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Domain.Posts.Entities;
using IPhotoProjector = CloudCanvas.Application.Posts.Photos.Interfaces.IPhotoProjectionStore;

namespace CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;

public sealed record CreatePhotoCommandHandler(IPhotoProjector store, IPhotoRepository ctx, IMessageBuilder mBuilder, IMessenger messenger) : IRequestHandler<CreatePhotoCommand, CreatePhotoResult>
{
    private readonly IPhotoProjector _store = store;
    private readonly IPhotoRepository _context = ctx;
    private readonly IMessenger _messenger = messenger;

    public async Task<CreatePhotoResult> Handle(CreatePhotoCommand command, CancellationToken cancellation = default)
    {   // Create & Save Photo
        Photo photo = command.ToPhoto();
        photo.Id = await _context.SaveAsync(photo, cancellation);
        // Create & Save Projection
        var projection = photo.ToProjection(command.Creator ?? throw new ArgumentNullException("Creator is null"));
        projection = await _store.SaveProjectionAsync(projection, Projection.Containers.UserPhotos, false, cancellation);
        var res = new CreatePhotoResult { Success = true, Photo = projection, OriginalContainer = command.ContainerName };
        // Announce: projection ready for thumbnails
        await _messenger.NofityProjectionCompletedAsync(res.OriginalContainer, projection, projection.Id!, cancellation);
        // Hide Id and Username for now
        projection.Creator!.SetDisplayNameOnly(projection.Creator.DisplayName);
        return res;
    }
}