using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using ICosmosRepo = CloudCanvas.Application.Abstractions.Persistence.IPostsRepository<CloudCanvas.Domain.Posts.Contracts.IPost>;
using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotos;
using Mapster;

namespace CloudCanvas.Application.Posts.Queries.GetAllPosts
{
    public record GetAllPostsQuery
    {
        public string? UserId { get; set; }
        public string? ContainerName { get; set; } = CloudCosmos.Containers.BlobMeta;
        public PostClassification Type { get; set; } = PostClassification.Photo;
    }

    public record GetAllPhotosQuery: GetAllPostsQuery
    {
    }

    public sealed class GetAllPostsRequestHandler(ICosmosRepo client)
    {
        private readonly ICosmosRepo _cosmos = client;

        public async Task<GetAllPhotosQueryResult> Handle(GetAllPostsQuery query)
        {
            var posts = await _cosmos.GetPhotosAsync(query?.ContainerName ?? CloudCosmos.Containers.BlobMeta);
            var res = new GetAllPhotosQueryResult(posts);

            return res;
        }

        public GalleryDTO SetGalleryDetails(IPost fromObject, GalleryDTO toObject, bool force = false)
        {
            if (force || fromObject.Classification == PostClassification.Gallery)
            {
                toObject.DisplayName = ((Gallery)fromObject).DisplayName;
                toObject.Description = ((Gallery)fromObject).Description;
                toObject.UserTags = ((Gallery)fromObject).UserTags;
            }
            return toObject;
        }

        private ReactionsOverviewDTO GetReactionsOverview(IPost fromObject)
        {
            return new()
            {
                Likes = fromObject.LikesCount(),
                Dislikes = fromObject.DisLikesCount(),
                EmojiReactions = fromObject.EmojiReactions.Count
            };
        }

        private void SetIdentityDetails(IPost fromObject, PostDTO toObject)
        {
            toObject.Id = fromObject.Id;
            toObject.Creator = new Creator
            {
                Id = fromObject.UserId!
            };
            toObject.CreatedOn = fromObject.CreatedOn;
            toObject.DeletedOn = fromObject.DeletedOn;
        }

        public PhotoDTO SetPhotoDetails(IPost fromObject, PhotoDTO toObject, bool force = false)
        {
            if(force || fromObject.Classification == PostClassification.Photo) {
                toObject.OriginalFilename = ((Photo)fromObject).OriginalFilename;
                toObject.Title = ((Photo)fromObject).Title;
                toObject.ContentLength = fromObject.ContentLength;
                toObject.Location = fromObject.Url?? throw new ArgumentException("Invalid image location."); 
                toObject.UserTags = ((Photo)fromObject).UserTags;
            }
            return toObject;
        }
    }
}
