using CloudCanvas.Application.Posts.Photos.Interfaces;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser
{
    public sealed class GetUserPhotosQueryHandler(IPhotoProjectionStore projectionStore): IRequestHandler<GetUserPhotosQuery, GetUserPhotosResult>
    {
        private IPhotoProjectionStore _store = projectionStore;
        public async Task<GetUserPhotosResult> Handle(GetUserPhotosQuery query, CancellationToken cancellation = default)
        {
            var posts = await _store.GetByUserIdAsync(query.UserId, cancellation);
            return new GetUserPhotosResult(posts);
        }
    }
}
