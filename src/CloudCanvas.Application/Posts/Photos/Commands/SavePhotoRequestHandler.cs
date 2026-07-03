using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Posts;
using Mapster;
using ICosmosRepo = CloudCanvas.Application.Abstractions.Persistence.IPostsRepository<CloudCanvas.Domain.Posts.Contracts.IPost>;

namespace CloudCanvas.Application.Posts.Photos.Commands;

public sealed record SavePhotoRequestHandler(ICosmosRepo client)
{
    private readonly ICosmosRepo _client = client;

    public async Task<SavePhotoQueryResult> Handle(SavePhotoCommand command)
    {
        var photo = command.Photo.Adapt<Photo>();
        photo.UserId = command.UserId;
        var result = await _client.SaveMetadataAsync(photo, CloudCosmos.Containers.BlobMeta);
        return new SavePhotoQueryResult
        {
            IsSuccessFull = result != null,
            Photo = photo.Adapt<PhotoDTO>()
        };
    }
}