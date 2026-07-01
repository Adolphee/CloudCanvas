using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.DTOs;
using Mapster;
using ICosmosRepo = CloudCanvas.Application.Abstractions.Persistence.IPostsRepository<CloudCanvas.Domain.Posts.Contracts.IPost>;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotos
{
    public record GetUserPhotosRequestHandler(ICosmosRepo cosmos)
    {
        private ICosmosRepo _cosmos = cosmos;
        public async Task<GetUserPhotosQueryResult> Handle(GetUserPhotosQuery query)
        {
            var posts = await _cosmos.GetUserPhotosAsync(query.UserId, query?.ContainerName ?? CloudCosmos.Containers.BlobMeta);
            return new GetUserPhotosQueryResult(posts);
        }
    }
}
