using CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Thumbnail;
using static CloudCanvas.Application.Common.Constants.BStorage;

namespace CloudCanvas.Application.Common.Mapping
{
    // Custom mapping class for converting between CloudCanvas application models and domain models
    public static class CCMapper
    {
        #region CONVERT TO DOMAIN MODELS
        public static Photo ToPhoto(this CreatePhotoCommand cmd)
        {
            return new()
            {
                Id = cmd.Id ?? Guid.NewGuid().ToString(),
                OriginalFilename = cmd.OriginalFilename,
                Title = cmd.Title,
                Caption = cmd.Caption,
                UserTags = cmd.UserTags ?? default!,
                ContentLength = cmd.ContentLength,
                Location = cmd.Location,
                UserId = cmd.UserId!,
                Classification = PostClassification.Photo,
                Thumbnails = new List<PhotoThumbnail>(),
                CreatedOn = cmd.CreatedOn,
                ModifiedOn = cmd.ModifiedOn,
                DeletedOn = cmd.DeletedOn,
                CommentsEnabled = cmd.CommentsEnabled
            };
        }
        public static Photo ToPhoto(this PhotoDTO dto)
        {
            return new()
            {
                Id = dto.Id,
                OriginalFilename = dto.OriginalFilename,
                Title = dto.Title,
                Caption = dto.Description,
                UserTags = dto.UserTags ?? default!,
                ContentLength = dto.ContentLength,
                Location = dto.Location,
                UserId = dto.UserId!,
                Classification = PostClassification.Photo,
                Thumbnails = dto.Thumbnails.Select(t => new PhotoThumbnail
                {
                    PhotoId = dto.Id,
                    OriginalImageURL = dto.Location,
                    Size = (ThumbnailSize) Enum.Parse(typeof(ThumbnailSize), t.Key.ToString()),
                    Url = t.Value
                }).ToList(),
                CreatedOn = dto.TimeStamps.CreatedOn,
                ModifiedOn = dto.TimeStamps.ModifiedOn,
                DeletedOn = dto.TimeStamps.DeletedOn,
                CommentsEnabled = dto.CommentsEnabled
            };
        }
        public static Photo ToPhoto(this FileMetadata fmdt, string userId)
        {
            var thumb = fmdt.Thumbnails;
            return new Photo
            {
                Id = fmdt.Id,
                OriginalFilename = fmdt.OriginalFilename,
                Title = fmdt.OriginalFilename,
                Caption = fmdt.Description,
                UserTags = fmdt.UserTags ?? default!,
                ContentLength = fmdt.ContentLength,
                Location = fmdt.Location,
                UserId = fmdt.Metadata.TryGetValue(Meta.UploadedBy, out var uploadedBy) ? uploadedBy : fmdt.UserId ?? userId,
                Classification = PostClassification.Photo,
                Thumbnails = thumb.Select(t => new PhotoThumbnail
                {
                    PhotoId = fmdt.Id,
                    OriginalImageURL = fmdt.Location,
                    Size = t.Key,
                    Url = t.Value
                }).ToList(),
                CreatedOn = fmdt.CreatedOn,
                ModifiedOn = fmdt.LastModified,
                DeletedOn = fmdt.DeletedOn,
                CommentsEnabled = fmdt.CommentsEnabled ?? false
            };
        }
        #endregion

        #region CONVERT TO DTOs
        public static PhotoDTO ToPhotoDTO(this FileMetadata fmdt, Creator creator)
        {
            return new PhotoDTO
            {
                Id = fmdt.Id,
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
        }
        public static PhotoDTO ToProjection(this Photo photo, Creator creator) {
            PhotoDTO photoDTO = new()
            {
                Id = photo.Id,
                UserId = photo.UserId, 
                OriginalFilename = photo.OriginalFilename,
                Location = photo.Location ?? throw new ArgumentNullException("Photo.Location is null while converting to PhotoDTO"),
                CommentsEnabled = photo.CommentsEnabled,
                Description = photo.Caption,
                Title = photo.Title,
                ContentLength = photo.ContentLength,
                UserTags = photo.UserTags,
                Thumbnails = photo.Thumbnails.ToDictionary(p => p.Size.ToString(), p => p.Url),
                GalleryId = photo.GalleryId,
                Reactions = new()
                {
                    Likes = photo.LikesCount(),
                    Dislikes = photo.DislikesCount(),
                    EmojiReactions = photo.EmojiReactionsCount()
                }, 
                Classification = photo.Classification.ToString(),
                Creator = creator ?? new()
                {
                    Id = photo.UserId,
                    UserName = "unknown_user",
                    DisplayName = "Unknown User"
                },
                TimeStamps = new Domain.Common.TimeStampz
                {
                    CreatedOn = photo.CreatedOn,
                    ModifiedOn = photo.ModifiedOn,
                    DeletedOn = photo.DeletedOn
                }, 
            };

            return photoDTO;
        }
        #endregion

        #region CONVERT TO COMMANDS
        public static CreatePhotoCommand IssueCreationCommand(this Photo photo, Creator creator)
        {
            return new()
            {
                Id = photo.Id ?? Guid.NewGuid().ToString(),
                Caption = photo.Caption,
                Classification = photo.Classification,
                CommentsEnabled = photo.CommentsEnabled,
                ContentLength = photo.ContentLength,
                CreatedOn = photo.CreatedOn,
                Creator = creator ?? throw new ArgumentNullException("Creator is null."),
                DeletedOn = photo.DeletedOn,
                GalleryId = photo.GalleryId,
                Location = photo.Location ?? throw new ArgumentNullException("Location is null."),
                OriginalFilename = photo.OriginalFilename,
                Title = photo.Title ?? throw new ArgumentNullException("Title is null."),
                UserId = photo.UserId,
                UserTags = photo.UserTags,
                TimeStamps = new()
                {
                    ModifiedOn = photo.ModifiedOn,
                    DeletedOn = photo.DeletedOn,
                        CreatedOn = photo.CreatedOn
                }
            };
        }
        #endregion
    }
}
