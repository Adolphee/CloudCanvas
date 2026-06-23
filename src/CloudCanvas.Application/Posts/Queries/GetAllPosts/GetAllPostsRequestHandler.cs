using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Reactions;
using System.Runtime.InteropServices.Marshalling;
using IPost = CloudCanvas.Domain.Posts.Contracts.IPost;
using ICosmos = CloudCanvas.Application.Abstractions.Persistence.IPostsRepository<CloudCanvas.Domain.Posts.Post>;
using IContext = CloudCanvas.Application.Abstractions.Persistence.IPostsRepository<CloudCanvas.Domain.Posts.Post>;
using CloudCanvas.Application.Reactions.Common;

namespace CloudCanvas.Application.Posts.Queries.GetAllPosts
{
    public record GetAllPostsQuery
    {
        public required string? UserId { get; set; }
        public required string? ContainerName { get; set; }
        public PostClassification Type { get; set; }
    }

    public sealed class GetAllPostsRequestHandler(ICosmos client, IContext context)
    {
        private readonly ICosmos _cosmos = client;
        private readonly IContext _context = context;

        public async Task<GetAllPhotosQueryResult> Handle(GetAllPostsQuery query)
        {
            var posts = await _cosmos.GetPostsAsync(query?.ContainerName ?? CloudCosmos.Containers.BlobMeta);
            var res = new GetAllPhotosQueryResult();
            posts.ForEach(p =>
            {
                PhotoDTO finalPost = new();
                finalPost = SetPhotoDetails(p, new PhotoDTO());
                SetIdentityDetails(p, finalPost);
                finalPost.Reactions = GetReactionsOverview(p);
                res.Posts.Add(finalPost);
            });

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
