using CloudCanvas.Application.Abstractions.Messaging;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Domain.Posts.Entities;
using IProjectionStore = CloudCanvas.Application.Posts.Photos.Interfaces.IPhotoProjectionStore;

namespace CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;

public sealed class CreatePhotoCommandHandler(IProjectionStore store, IPhotoRepository repostitory, IMessenger messenger) : IRequestHandler<CreatePhotoCommand, CreatePhotoResult>
{
    private readonly IProjectionStore _store = store;
    private readonly IPhotoRepository _repostitory = repostitory;
    private readonly IMessenger _messenger = messenger;

    public async Task<CreatePhotoResult> Handle(CreatePhotoCommand command, CancellationToken cancellation = default)
    {   // Create & Save Photo
        Photo photo = command.ToPhoto();
        photo.Id = await _repostitory.SaveAsync(photo, cancellation);
        // Create & Save Projection
        var projection = photo.ToProjection(command.Creator);
        projection = await _store.CreateProjectionAsync(projection, cancellation);
        var res = new CreatePhotoResult(projection);
        // Announce: projection ready
        await _messenger.NofityProjectionCompletedAsync(projection, projection.Id!, cancellation);
        return res;
    }
}