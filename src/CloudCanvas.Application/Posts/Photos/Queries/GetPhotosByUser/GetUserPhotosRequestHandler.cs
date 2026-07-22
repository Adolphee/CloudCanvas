using CloudCanvas.Application.Posts.Photos.Interfaces;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser
{
    public record GetUserPhotosRequestHandler(IPhotoProjectionStore cosmos): IRequestHandler<GetUserPhotosQuery, GetUserPhotosResult>
    {
        private IPhotoProjectionStore _cosmos = cosmos;
        public async Task<GetUserPhotosResult> Handle(GetUserPhotosQuery query, CancellationToken cancellation = default)
        {
            var posts = await _cosmos.GetByUserIdAsync(query.UserId, query?.ContainerName ?? CloudCosmos.Containers.UserPhotos, cancellation);
            return new GetUserPhotosResult(posts);
        }
    }
}
