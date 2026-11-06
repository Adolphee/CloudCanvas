using CloudCanvas.Application.Posts.Comments;
using CloudCanvas.Application.Posts.Comments.Commands.EditComment;
using CloudCanvas.Domain.Posts.Entities;

namespace CloudCanvas.Application.Common.Mapping
{
    public static class CommentMappings
    {
        public static CommentDTO ToProjection(this Comment comment, string AuthorDisplayName = Unknown.DisplayName)
        {
            return new()
            {
                Id = comment.Id,
                Text = comment.Text,
                Username = comment.UserDisplayName ?? AuthorDisplayName,
                UserId = comment.UserId,
                CreatedOn = comment.CreatedOn,
                ModifiedOn = comment.ModifiedOn
            };
        }

        public static EditCommentCommand ToEditCommentCommand(this Comment comment)
        {
            return new()
            {
                Id = comment.Id,
                Text = comment.Text,
                UserId = comment.UserId,
                PostId = comment.PostId,
                UserDisplayName = comment.UserDisplayName
            };
        }
    }
}