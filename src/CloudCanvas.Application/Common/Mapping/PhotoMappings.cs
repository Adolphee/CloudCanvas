using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;
using CloudCanvas.Domain.Enums;
using CloudCanvas.Domain.Posts.Entities;
using CloudCanvas.Domain.Thumbnail;
using static CloudCanvas.Application.Common.Constants.BStorage;

namespace CloudCanvas.Application.Common.Mapping
{
    public static class PhotoMappings
    {
        public static Photo ToPhoto(this CreatePhotoCommand cmd) => new()
        {
            Id = cmd.Id ?? Guid.NewGuid().ToString(),
            OriginalFilename = cmd.OriginalFilename,
            Title = cmd.Title,
            Caption = cmd.Caption,
            UserTags = cmd.UserTags ?? default!,
            ContentLength = cmd.ContentLength,
            Location = cmd.Location,
            UserId = cmd.UserId!,
            Thumbnails = [],
            CreatedOn = DateTimeOffset.Now,
            ModifiedOn = default,
            DeletedOn = default,
            CommentsEnabled = cmd.CommentsEnabled
        };
        public static Photo ToPhoto(this PhotoDTO dto, bool assignThumbnaiIds = true) => new()
        {
            Id = dto.Id ?? Guid.NewGuid().ToString(),
            OriginalFilename = dto.OriginalFilename,
            Title = dto.Title,
            Caption = dto.Description,
            UserTags = dto.UserTags ?? [],
            ContentLength = dto.ContentLength,
            Location = dto.Location,
            UserId = dto.UserId!,
            Thumbnails = [.. dto.Thumbnails.Select(t => new PhotoThumbnail
            {
                Id = assignThumbnaiIds? Guid.NewGuid().ToString(): default,
                PhotoId = dto.Id,
                OriginalImageURL = dto.Location,
                Size = Enum.Parse<ThumbnailSize>(t.Key.ToString()),
                Url = t.Value
            })],
            CreatedOn = dto.TimeStamps.CreatedOn,
            ModifiedOn = dto.TimeStamps.ModifiedOn,
            DeletedOn = dto.TimeStamps.DeletedOn,
            CommentsEnabled = dto.CommentsEnabled
        };
        public static Photo ToPhoto(this FileMetadata fmdt, string userId) => new()
        {
            Id = fmdt.Id ?? Guid.NewGuid().ToString(),
            OriginalFilename = fmdt.OriginalFilename,
            Title = fmdt.OriginalFilename,
            Caption = fmdt.Description,
            UserTags = fmdt.UserTags ?? default!,
            ContentLength = fmdt.ContentLength,
            Location = fmdt.Location,
            UserId = fmdt.Metadata.TryGetValue(Meta.UploadedBy, out var uploadedBy) ? uploadedBy : fmdt.UserId ?? userId,
            Thumbnails = [.. fmdt.Thumbnails.Select(t => new PhotoThumbnail
            {
                PhotoId = fmdt.Id ?? Guid.NewGuid().ToString(),
                OriginalImageURL = fmdt.Location,
                Size = t.Key,
                Url = t.Value
            })],
            CreatedOn = fmdt.CreatedOn,
            ModifiedOn = fmdt.LastModified,
            DeletedOn = fmdt.DeletedOn,
            CommentsEnabled = fmdt.CommentsEnabled ?? false
        };
        
        public static PhotoDTO ToProjection(this FileMetadata fmdt, Creator creator) => new()
        {
            Id = fmdt.Id ?? Guid.NewGuid().ToString(),
            OriginalFilename = fmdt.OriginalFilename,
            Title = fmdt.OriginalFilename,
            Description = fmdt.Description,
            UserTags = fmdt.UserTags ?? default!,
            ContentLength = fmdt.ContentLength,
            Location = fmdt.Location,
            Classification = nameof(PostClassification.Photo),
            Thumbnails = fmdt.Thumbnails.ToDictionary(k => k.ToString(), v => v.Value),
            UserId = fmdt.Metadata.TryGetValue(Meta.UploadedBy, out var uploadedBy) ? uploadedBy : fmdt.UserId ?? creator.GetId(),
            Creator = creator,
            TimeStamps = new()
            {
                CreatedOn = fmdt.CreatedOn,
                ModifiedOn = fmdt.LastModified,
                DeletedOn = DateTimeOffset.MinValue
            },
            CommentsEnabled = fmdt.CommentsEnabled ?? false
        };
        public static PhotoDTO ToProjection(this Photo photo, Creator creator) => new()
        {
            Id = photo.Id ?? Guid.NewGuid().ToString(),
            UserId = photo.UserId,
            OriginalFilename = photo.OriginalFilename,
            Location = photo.Location ?? throw new CCMapperException("Photo.Location is null"),
            CommentsEnabled = photo.CommentsEnabled,
            Description = photo.Caption,
            Title = photo.Title,
            ContentLength = photo.ContentLength,
            UserTags = photo.UserTags,
            GalleryId = photo.GalleryId,
            Reactions = {
                Likes = photo.LikesCount(),
                Dislikes = photo.DislikesCount(),
                EmojiReactions = photo.EmojiReactionsCount()
            },
            Creator = creator ?? new()
            {
                Id = photo.UserId,
                UserName = Unknown.Username,
                DisplayName = Unknown.DisplayName
            },
            TimeStamps = new Domain.Abstractions.AuditableEntity
            {
                CreatedOn = photo.CreatedOn,
                ModifiedOn = photo.ModifiedOn,
                DeletedOn = photo.DeletedOn
            },
            Thumbnails = photo.Thumbnails.ToDictionary(p => p.Size.ToString(), p => p.Url)
        };

    }
}
