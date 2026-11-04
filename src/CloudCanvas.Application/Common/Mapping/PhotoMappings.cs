using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;
using CloudCanvas.Domain.Abstractions;
using CloudCanvas.Domain.Enums;
using CloudCanvas.Domain.Posts.Entities;
using CloudCanvas.Domain.Thumbnail;
using static CloudCanvas.Application.Common.Constants.BStorage;

namespace CloudCanvas.Application.Common.Mapping
{
    public static class PhotoMappings
    {

        public static PhotoDTO ToProjection(this FileMetadata fmdt, CreatorMinimal creator)
        {
            PhotoDTO dto = new()
            {
                Id = fmdt.Id ?? Guid.NewGuid().ToString(),
                OriginalFilename = fmdt.OriginalFilename,
                Title = fmdt.OriginalFilename,
                Description = fmdt.Description,
                UserTags = fmdt.UserTags ?? default!,
                ContentLength = fmdt.ContentLength,
                Location = fmdt.Location,
                Thumbnails = fmdt.Thumbnails.ToDictionary(k => k.ToString(), v => v.Value),
                UserId = fmdt.Metadata.TryGetValue(Meta.UploadedBy, out var uploadedBy) ? uploadedBy : fmdt.UserId ?? creator.Id,
                Creator = creator,
                TimeStamps = new()
                {
                    CreatedOn = fmdt.CreatedOn
                },
                CommentsEnabled = fmdt.CommentsEnabled ?? false
            };
            dto.TimeStamps.SetModifiedOn(fmdt.LastModified);
            dto.TimeStamps.SetDeletedOn(fmdt.DeletedOn);
            return dto;
        }

        public static PhotoDTO ToProjection(this Photo photo, CreatorMinimal creator)
        {
            PhotoDTO dto = new()
            {
                Id = photo.Id ?? Guid.NewGuid().ToString(),
                UserId = photo.UserId,
                OriginalFilename = photo.OriginalFilename,
                Location = photo.Location ?? throw new CCMapperException("Photo.Location is null"),
                CommentsEnabled = photo.CommentsEnabled,
                Description = photo.Caption ?? photo.SmartCaption,
                SmartCaption = photo.SmartCaption,
                Title = photo.Title,
                ContentLength = photo.ContentLength,
                UserTags = photo.UserTags,
                SmartTags = photo.SmartTags,
                GalleryId = photo.GalleryId,
                Reactions = {
                    Likes = photo.LikesCount(),
                    Dislikes = photo.DislikesCount(),
                    EmojiReactions = photo.EmojiReactionsCount()
                },
                Creator = creator,
                TimeStamps = new TimeStamps
                {
                    CreatedOn = photo.CreatedOn
                },
                Thumbnails = photo.Thumbnails.ToDictionary(p => p.Size.ToString(), p => p.Url)
            };
            dto.TimeStamps.SetModifiedOn(photo.ModifiedOn);
            dto.TimeStamps.SetDeletedOn(photo.DeletedOn);
            return dto;
        }


        public static Photo ToPhoto(this CreatePhotoCommand cmd)
        {
            Photo p = new()
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
                CommentsEnabled = cmd.CommentsEnabled
            };
            p.SetModifiedOn(DateTimeOffset.Now);
            p.SetDeletedOn(DateTimeOffset.MinValue);
            return p;
        }

        public static Photo ToPhoto(this PhotoDTO dto, bool assignThumbnaiIds = true)
        {
            Photo p = new()
            {
                Id = dto.Id ?? Guid.NewGuid().ToString(),
                OriginalFilename = dto.OriginalFilename,
                Title = dto.Title,
                Caption = dto.Description,
                SmartCaption = dto.SmartCaption,
                UserTags = dto.UserTags ?? dto.UserTags ?? [],
                SmartTags = dto.SmartTags?.Select(st => new SmartTag
                {
                    Id = st.Id ?? Guid.NewGuid().ToString(),
                    Name = st.Name,
                    Confidence = st.Confidence
                }).ToList() ?? [],
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
                CommentsEnabled = dto.CommentsEnabled
            };
            p.SetModifiedOn(DateTimeOffset.Now);
            p.SetDeletedOn(DateTimeOffset.MinValue);
            return p;
        }

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
            CommentsEnabled = fmdt.CommentsEnabled ?? false
        };

        public static EnrichmentTarget ToEnrichmentTarget(this PhotoDTO photo) => new(photo.Id!, photo.UserId!, photo.Location) { tags = photo.SmartTags };

        // CONVERT TO COMMANDS
        public static CreatePhotoCommand IssueCreationCommand(this Photo photo, Creator creator) => new()
        {
            Id = photo.Id ?? Guid.NewGuid().ToString(),
            Caption = photo.Caption,
            CommentsEnabled = photo.CommentsEnabled,
            ContentLength = photo.ContentLength,
            Creator = creator ?? throw new ArgumentNullException(nameof(creator)),
            GalleryId = photo.GalleryId,
            Location = photo.Location ?? throw new ArgumentNullException(nameof(photo), "Photo.Location cannot be null"),
            OriginalFilename = photo.OriginalFilename,
            Title = photo.Title ?? throw new ArgumentNullException(nameof(photo), "Photo.Title cannot be null."),
            UserId = photo.UserId,
            UserTags = photo.UserTags
        };
    }
}