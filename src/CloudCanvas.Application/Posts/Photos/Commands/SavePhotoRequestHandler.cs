using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.Contracts;
using Mapster;
using ICosmosRepo = CloudCanvas.Application.Abstractions.Cosmos.IPostsRepositoryCosmos<CloudCanvas.Domain.Posts.Contracts.IPost>;

namespace CloudCanvas.Application.Posts.Photos.Commands;

public sealed record SavePhotoRequestHandler(ICosmosRepo client, IPhotoRepositoryEF ctx)
{
    private readonly ICosmosRepo _client = client;
    private readonly IPhotoRepositoryEF _context = ctx;

    public async Task<SavePhotoQueryResult> Handle(SavePhotoCommand command, CancellationToken cancellation = default)
    {
        command.SetCreatedOn();

        var cosmosPhoto = command.Adapt<PhotoDTO>();
        cosmosPhoto.UserId = command.UserId;
        cosmosPhoto.Creator = new Creator
        {
            Id = command.UserId,
            DisplayName = "Unknown User",
            UserName = "@anon12345"
        };
        
        Photo efPhoto = new()
        {
            Id = cosmosPhoto.Id,
            Location = cosmosPhoto.Location,
            Caption = cosmosPhoto.Description,
            ContentLength = cosmosPhoto.ContentLength,
            CreatedOn = cosmosPhoto.TimeStamps.CreatedOn,
            GalleryId = cosmosPhoto.Id,
            OriginalFilename = cosmosPhoto.OriginalFilename,
            Title = cosmosPhoto.Title,
            UserId = cosmosPhoto.UserId,
            UserTags = cosmosPhoto.UserTags ?? new()
        };


        cosmosPhoto.Id = await _context.AddPhotoAsync(efPhoto, cancellation);
        
        var result = await _client.SavePhotoAsync(cosmosPhoto, CloudCosmos.Containers.UserPhotos, false);
        return new SavePhotoQueryResult
        {
            IsSuccessFull = result != null,
            Photo = result
        };
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

    private void SetIdentityDetails(Post fromObject, PostDTO toObject)
    {
        toObject.Id = fromObject.Id;
        toObject.Creator = new Creator
        {
            Id = fromObject.UserId!
        };
        toObject.TimeStamps.CreatedOn = fromObject.CreatedOn;
        toObject.TimeStamps.DeletedOn = fromObject.DeletedOn;
    }

}