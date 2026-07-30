using CloudCanvas.Application.Posts.Photos.Interfaces;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetAllPhotos
{

    public sealed class GetAllPhotosQueryHandler(IPhotoProjectionStore store): IRequestHandler<GetAllPhotosQuery, GetAllPhotosResult>
    {
        private readonly IPhotoProjectionStore _store = store;

        public async Task<GetAllPhotosResult> Handle(GetAllPhotosQuery query, CancellationToken cancellationToken = default)
        {
            return new GetAllPhotosResult(await _store.GetAllAsync(cancellationToken));
        }
    }
}
