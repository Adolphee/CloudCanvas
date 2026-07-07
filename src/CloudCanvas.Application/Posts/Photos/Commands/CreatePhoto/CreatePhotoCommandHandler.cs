using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.Contracts;
using Mapster;
using MediatR;
using IPhotoProjection = CloudCanvas.Application.Posts.Photos.Interfaces.IPhotoProjectionStore;

namespace CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;

public sealed record CreatePhotoCommandHandler(IPhotoProjection store, IPhotoRepository ctx): IRequestHandler<CreatePhotoCommand, CreatePhotoResult>
{
    private readonly IPhotoProjection _store = store;
    private readonly IPhotoRepository _context = ctx;

    public async Task<CreatePhotoResult> Handle(CreatePhotoCommand command, CancellationToken cancellation = default)
    {
        var cosmosPhoto = command.Adapt<PhotoDTO>();
        cosmosPhoto.UserId = command.UserId;
        
        Photo efPhoto = new()
        {
            Id = Guid.NewGuid().ToString(),
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
        cosmosPhoto.Id = await _context.SaveAsync(efPhoto, cancellation);
        cosmosPhoto = await _store.SaveProjectionAsync(cosmosPhoto, CloudCosmos.Containers.UserPhotos, false);
        cosmosPhoto.Creator!.SetDisplayNameOnly(cosmosPhoto.Creator.DisplayName);
        return new CreatePhotoResult { Success = true, Photo = cosmosPhoto };
    }
}