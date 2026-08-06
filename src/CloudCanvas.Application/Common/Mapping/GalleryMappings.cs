using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Application.Posts.Galleries;
using CloudCanvas.Application.Posts.Galleries.Commands.CreateGallery;
using CloudCanvas.Domain.Enums;
using CloudCanvas.Domain.Posts.Entities;

namespace CloudCanvas.Application.Common.Mapping
{
    public static class GalleryMappings
    {
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

        public static GalleryItemDTO ToGalleryItem(this Photo photo, Creator creator) => new()
        {
            Location = photo.Location ?? throw new CCMapperException("Photo.Location is null"),
            Title = photo.Title ?? throw new CCMapperException("Photo.Title is null"),
            MediumThumbnail = photo.Thumbnails.FirstOrDefault(t => t.Size == ThumbnailSize.medium)?.Url,
            Creator = creator ?? new(photo.UserId, Unknown.Username, Unknown.DisplayName)
        };
    }
}
