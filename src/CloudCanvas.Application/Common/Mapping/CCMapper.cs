using CloudCanvas.Application.Posts.Comments;
using CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;
using CloudCanvas.Domain.Posts.Entities;

namespace CloudCanvas.Application.Common.Mapping
{
    // Custom mapping class for converting between CloudCanvas app models and domain models
    public static class CCMapper
    {
        public static CommentDTO ToProjection(this Comment comment, string AuthorDisplayName = default!) => new()
        {
            Text = comment.Text,
            AuthorDisplayName = AuthorDisplayName,
            CreatedOn = comment.CreatedOn,
            //Reactions = comment.Reactions.ToProjection()
        };
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
