using CloudCanvas.Application.Abstractions.Messaging;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Domain.Posts;
using IPhotoProjector = CloudCanvas.Application.Posts.Photos.Interfaces.IPhotoProjectionStore;

namespace CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;

public sealed record CreatePhotoCommandHandler(IPhotoProjector store, IPhotoRepository ctx, IMessageBuilder mBuilder, IMessenger messenger, IMessageFactory mFactory) : IRequestHandler<CreatePhotoCommand, CreatePhotoResult>
{
    private readonly IPhotoProjector _store = store;
    private readonly IPhotoRepository _context = ctx;
    private readonly IMessenger _messenger = messenger;
    private readonly IMessageFactory _mFactory = mFactory;

    public async Task<CreatePhotoResult> Handle(CreatePhotoCommand command, CancellationToken cancellation = default)
    {   // Create & Save Photo
        Photo photo = command.ToPhoto();
        photo.Id = await _context.SaveAsync(photo, cancellation);
        // Create & Save Projection
        var projection = photo.ToProjection(command.Creator ?? throw new ArgumentNullException("Creator is null"));
        projection = await _store.SaveProjectionAsync(projection, CloudCosmos.Containers.UserPhotos, false, cancellation);
        // Announce: projection ready for thumbnails
        var res = await _messenger.NofityProjectionCompletedAsync(projection, projection.Id!, cancellation);
        // Hide Id and Username for now
        projection.Creator!.SetDisplayNameOnly(projection.Creator.DisplayName);
        return new(){ Success = true, Photo = projection };
    }
}