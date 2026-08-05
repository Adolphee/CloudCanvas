using CloudCanvas.Application.Abstractions.Identity;
using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Application.Posts.Galleries;
using CloudCanvas.Application.Posts.Galleries.Commands.CreateGallery;
using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;
using CloudCanvas.Domain.Enums;
using CloudCanvas.Domain.Posts.Entities;
using CloudCanvas.Domain.Thumbnail;
using System.Security.Claims;
using static CloudCanvas.Application.Common.Constants.BStorage;

namespace CloudCanvas.Application.Common.Mapping
{
    // Custom mapping class for converting between CloudCanvas app models and domain models
    public static class CCMapper
    {
        #region CONVERT TO DOMAIN MODELS
        
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

        public static Gallery ToGallery(this CreateGalleryCommand command, CancellationToken cancellation = default)
        {
            var gallery = new Gallery
            {
                Id = command.Id ?? Guid.NewGuid().ToString(),
                UserId = command.UserId,
                DisplayName = command.DisplayName,
                Description = command.Description,
                CommentsEnabled = command.CommentsEnabled,
                Photos = new List<Photo>()
            };
            return gallery;
        }
        #endregion

        #region CONVERT TO DTOs
        public static Creator ToCreator(this ApplicationUser user) => new(user.Id, user.UserName, user.DisplayName);
        public static ApplicationUser ToAppUser(this ClaimsPrincipal prinicipal) => new()
        {
            Id = prinicipal.FindFirstValue(CCClaimTypes.ObjectIdentfier)!,
            Email = prinicipal.FindFirstValue(ClaimTypes.Email)!,
            FirstName = prinicipal.FindFirstValue(ClaimTypes.GivenName)!,
            LastName = prinicipal.FindFirstValue(ClaimTypes.Surname)!,
            UserName = prinicipal.FindFirstValue(ClaimTypes.Email)!
        };
        public static PhotoDTO ToPhotoDTO(this FileMetadata fmdt, Creator creator) => new()
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

        public static GalleryItemDTO ToGalleryItem(this Photo photo, Creator creator) => new(){
            Location = photo.Location ?? throw new CCMapperException("Photo.Location is null"),
            Title = photo.Title ?? throw new CCMapperException("Photo.Title is null"),
            MediumThumbnail = photo.Thumbnails.FirstOrDefault(t => t.Size == ThumbnailSize.medium)?.Url,
            Creator = creator?? new(photo.UserId, Unknown.Username, Unknown.DisplayName)
        };

    public static GalleryDTO ToProjection(this Gallery gallery, Creator creator) => new(gallery.Id, creator)
        {
            Id = gallery.Id ?? Guid.NewGuid().ToString(),
            UserId = gallery.UserId,
            DisplayName = gallery.DisplayName,
            Description = gallery.Description,
            CommentsEnabled = gallery.CommentsEnabled,
            Creator = creator ?? new()
            {
                Id = gallery.UserId,
                UserName = Unknown.Username,
                DisplayName = Unknown.DisplayName
            },
            TimeStamps = new Domain.Abstractions.AuditableEntity
            {
                CreatedOn = gallery.CreatedOn,
                ModifiedOn = gallery.ModifiedOn,
                DeletedOn = gallery.DeletedOn
            },
            Photos = gallery.Photos.Select(p => p.ToGalleryItem(creator)).ToList(),
            Reactions = {
                Likes = gallery.LikesCount(),
                Dislikes = gallery.DislikesCount(),
                EmojiReactions = gallery.EmojiReactionsCount()
            }, 
            UserTags = gallery.UserTags
        };
        #endregion

        #region CONVERT TO COMMANDS
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

        #endregion
    }
}
